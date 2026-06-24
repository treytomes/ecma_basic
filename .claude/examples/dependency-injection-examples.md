# Dependency Injection Examples

**Related**: Issue #33 (DI Expansion Research), Issues #30, #41, #42

This document provides practical examples of using dependency injection in the ECMA BASIC interpreter.

## Table of Contents

1. [Creating Environments with Custom Services](#creating-environments-with-custom-services)
2. [Testing with Custom Configuration](#testing-with-custom-configuration)
3. [Testing with Seeded RNG](#testing-with-seeded-rng)
4. [Registering New Intrinsic Functions](#registering-new-intrinsic-functions)
5. [DI Container Setup](#di-container-setup)

---

## Creating Environments with Custom Services

### Basic Environment Creation

```csharp
// Default environment (uses MinimalBasicConfiguration.Instance fallback)
var env = new ConsoleEnvironment();

// Environment with custom config
var customConfig = new MinimalBasicConfiguration(myConfiguration);
var env = new ConsoleEnvironment(config: customConfig);
```

### Custom Environment for Testing

```csharp
public class TestEnvironment : EnvironmentBase
{
    private readonly StringWriter _output = new();
    private readonly StringReader _input;

    public TestEnvironment(
        Interpreter? interpreter = null,
        IBasicConfiguration? config = null,
        string? input = null)
        : base(interpreter, config)
    {
        _input = new StringReader(input ?? string.Empty);
    }

    public override int TerminalRow { get; set; }
    public override int TerminalColumn { get; set; }

    public override string ReadLine() => _input.ReadLine() ?? string.Empty;
    public override void Print(string text) => _output.Write(text);
    public override void PrintLine(string text = "") => _output.WriteLine(text);
    public override void ReportError(string message) => PrintLine($"ERROR: {message}");

    public string GetOutput() => _output.ToString();
}
```

---

## Testing with Custom Configuration

### Example 1: Custom String Length Limit

```csharp
[Fact]
public void StringAssignment_ExceedsCustomLimit_ThrowsException()
{
    // Arrange
    var configData = new Dictionary<string, string>
    {
        ["maxStringLength"] = "5"
    };
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();
    var customConfig = new MinimalBasicConfiguration(configuration);
    var env = new TestEnvironment(config: customConfig);

    // Act & Assert
    var exception = Assert.Throws<RuntimeException>(() =>
    {
        env.SetStringVariableValue("A$", "TOOLONG"); // 7 chars > 5 limit
    });
}
```

### Example 2: Custom Line Length

```csharp
[Fact]
public void ProgramLine_ExceedsCustomLength_IsRejected()
{
    // Arrange
    var configData = new Dictionary<string, string>
    {
        ["maxLineLength"] = "20"
    };
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();
    var customConfig = new MinimalBasicConfiguration(configuration);
    var env = new TestEnvironment(config: customConfig);

    // Assert
    Assert.Equal(20, env.Configuration.MaxLineLength);
}
```

### Example 3: Testing Different Terminal Widths

```csharp
[Theory]
[InlineData(40, 4)]  // 40 chars, 4 columns
[InlineData(80, 5)]  // 80 chars, 5 columns
[InlineData(132, 6)] // 132 chars, 6 columns
public void PrintZones_RespectTerminalWidth(int terminalWidth, int numColumns)
{
    // Arrange
    var configData = new Dictionary<string, string>
    {
        ["terminalWidth"] = terminalWidth.ToString(),
        ["numTerminalColumns"] = numColumns.ToString()
    };
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();
    var customConfig = new MinimalBasicConfiguration(configuration);
    var env = new TestEnvironment(config: customConfig);

    // Assert
    Assert.Equal(terminalWidth, env.Configuration.TerminalWidth);
    Assert.Equal(numColumns, env.Configuration.NumTerminalColumns);
    Assert.Equal(terminalWidth / numColumns, env.Configuration.TerminalColumnWidth);
}
```

---

## Testing with Seeded RNG

### Example 1: Repeatable RND Sequence

```csharp
[Fact]
public void RND_WithSameSeed_ProducesSameSequence()
{
    // Arrange
    var env1 = new TestEnvironment();
    var env2 = new TestEnvironment();
    
    env1.Random.Reseed(42);
    env2.Random.Reseed(42); // Same seed

    // Act
    var sequence1 = Enumerable.Range(0, 100)
        .Select(_ => env1.Random.NextDouble())
        .ToList();
    var sequence2 = Enumerable.Range(0, 100)
        .Select(_ => env2.Random.NextDouble())
        .ToList();

    // Assert
    Assert.Equal(sequence1, sequence2);
}
```

### Example 2: Default Seed is Repeatable

```csharp
[Fact]
public void RND_WithDefaultSeed_IsRepeatable()
{
    // Arrange - Create two fresh environments (both use seed 42)
    var env1 = new TestEnvironment();
    var env2 = new TestEnvironment();

    // Act - Generate sequences without explicit re-seeding
    var first1 = env1.Random.NextDouble();
    var first2 = env2.Random.NextDouble();

    // Assert - Both should produce same first value (ECMA55-FUN-008)
    Assert.Equal(first1, first2);
}
```

### Example 3: Independent Environment State

```csharp
[Fact]
public void MultipleEnvironments_HaveIndependentRNG()
{
    // Arrange
    var env1 = new TestEnvironment();
    var env2 = new TestEnvironment();

    // Act - Advance env1's RNG state
    for (int i = 0; i < 10; i++)
    {
        env1.Random.NextDouble();
    }

    // Both environments started with same seed, but env1 advanced
    var value1 = env1.Random.NextDouble(); // 11th value
    var value2 = env2.Random.NextDouble(); // 1st value

    // Assert - Values are different (independent state)
    Assert.NotEqual(value1, value2);
}
```

### Example 4: Testing RANDOMIZE Behavior

```csharp
[Fact]
public void RANDOMIZE_ChangesRNGSequence()
{
    // Arrange
    var env = new TestEnvironment();
    
    // Get first value with default seed
    var firstValue = env.Random.NextDouble();

    // Re-seed with different value (simulates RANDOMIZE)
    env.Random.Reseed(12345);

    // Act
    var secondValue = env.Random.NextDouble();

    // Assert - Different seed = different sequence
    Assert.NotEqual(firstValue, secondValue);
}
```

---

## Registering New Intrinsic Functions

### Example 1: Simple Numeric Function

```csharp
// Register a SQUARE function that returns x²
env.Intrinsics.Register("SQUARE", 
    [ExpressionType.Number], 
    (env, args) =>
    {
        var x = Convert.ToDouble(args[0]);
        return x * x;
    });

// Use in BASIC:
// 10 LET A = SQUARE(5)
// 20 PRINT A
// Output: 25
```

### Example 2: Function Using Environment Services

```csharp
// Register a RANDF function that returns random float in [min, max)
env.Intrinsics.Register("RANDF",
    [ExpressionType.Number, ExpressionType.Number],
    (env, args) =>
    {
        var min = Convert.ToDouble(args[0]);
        var max = Convert.ToDouble(args[1]);
        return min + (env.Random.NextDouble() * (max - min));
    });

// Use in BASIC:
// 10 LET TEMP = RANDF(98.0, 100.0)
// 20 PRINT TEMP
// Output: 98.372... (random value between 98 and 100)
```

### Example 3: String Function

```csharp
// Register a REVERSE$ function that reverses a string
env.Intrinsics.Register("REVERSE$",
    [ExpressionType.String],
    (env, args) =>
    {
        var str = args[0].ToString() ?? string.Empty;
        return new string(str.Reverse().ToArray());
    });

// Use in BASIC:
// 10 LET A$ = "HELLO"
// 20 LET B$ = REVERSE$(A$)
// 30 PRINT B$
// Output: OLLEH
```

### Example 4: Function with Configuration Access

```csharp
// Register a MAXSTR function that returns the max string length config
env.Intrinsics.Register("MAXSTR",
    [],
    (env, args) => (double)env.Configuration.MaxStringLength);

// Use in BASIC:
// 10 PRINT MAXSTR
// Output: 18
```

---

## DI Container Setup

### Full Registration Example (ECMABasic55)

```csharp
private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
{
    // 1. Runtime Configuration (from appsettings.yaml)
    services.Configure<RuntimeConfiguration>(ctx.Configuration);
    services.AddSingleton<RuntimeConfiguration>(sp => 
        sp.GetRequiredService<IOptions<RuntimeConfiguration>>().Value);

    // 2. BASIC Configuration (injected from IConfiguration)
    services.AddSingleton<IBasicConfiguration>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        return new MinimalBasicConfiguration(configuration);
    });

    // 3. Interpreter (singleton, stateless)
    services.AddSingleton<Interpreter, RuntimeInterpreter>();

    // 4. Environment (creates per-environment services)
    services.AddSingleton<IEnvironment>(sp =>
    {
        var interpreter = (RuntimeInterpreter)sp.GetRequiredService<Interpreter>();
        var env = new ConsoleEnvironment(interpreter);
        
        // Inject non-ECMA-55 extensions
        InjectIntrinsics(env);
        
        return env;
    });
}
```

### Testing DI Container

```csharp
[Fact]
public void DIContainer_ResolvesAllServices()
{
    // Arrange
    var services = new ServiceCollection();
    
    services.AddSingleton<IBasicConfiguration>(
        new MinimalBasicConfiguration());
    services.AddSingleton<Interpreter, RuntimeInterpreter>();
    services.AddSingleton<IEnvironment>(sp =>
    {
        var interpreter = sp.GetRequiredService<Interpreter>();
        return new TestEnvironment(interpreter);
    });

    var provider = services.BuildServiceProvider();

    // Act
    var config = provider.GetRequiredService<IBasicConfiguration>();
    var interpreter = provider.GetRequiredService<Interpreter>();
    var environment = provider.GetRequiredService<IEnvironment>();

    // Assert
    Assert.NotNull(config);
    Assert.NotNull(interpreter);
    Assert.NotNull(environment);
    Assert.IsType<RuntimeInterpreter>(interpreter);
}
```

### Scoped Configuration for Different Dialects

```csharp
// Future: Support ECMA-55 and ECMA-116 dialects in same app
services.AddSingleton<IBasicConfiguration>(sp =>
{
    var dialect = sp.GetRequiredService<RuntimeConfiguration>().Dialect;
    
    return dialect switch
    {
        "ECMA-55" => new MinimalBasicConfiguration(sp.GetRequiredService<IConfiguration>()),
        "ECMA-116" => new ExtendedBasicConfiguration(sp.GetRequiredService<IConfiguration>()),
        _ => throw new ArgumentException($"Unknown dialect: {dialect}")
    };
});
```

---

## Best Practices

### ✅ DO

- **Use DI for services with state or configuration dependencies**
- **Create per-environment services for test isolation**
- **Use fixed seeds for deterministic testing**
- **Inject IConfiguration for custom configs**
- **Keep Domain layer DI-free (interfaces only)**

### ❌ DON'T

- **Don't use DI for pure functions** (use static methods)
- **Don't over-DI** (not everything needs injection)
- **Don't share environment instances** between tests
- **Don't modify global singletons** in tests
- **Don't break backward compatibility** (keep fallbacks)

---

## See Also

- [CLAUDE.md](../../CLAUDE.md) - DI guidelines section
- [dependency-injection.md](../../docs/architecture/dependency-injection.md) - Architecture overview
- Research: `.claude/research/dependency-injection-expansion.md` - Full analysis
