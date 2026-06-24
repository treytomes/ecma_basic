# Dependency Injection Architecture

**Last Updated**: 2026-06-24  
**Related Issues**: #30, #33, #41, #42

## Overview

The ECMA BASIC interpreter uses selective dependency injection - converting singletons and global state where it improves testability and architectural clarity, while keeping pure functions and simple patterns as-is.

## Dependency Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│                      (ECMABasic55)                          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ IHost Container                                       │  │
│  │  • RuntimeConfiguration (IOptions<T>)                │  │
│  │  • IBasicConfiguration (from IConfiguration)         │  │
│  │  • Interpreter (RuntimeInterpreter)                  │  │
│  │  • IEnvironment (ConsoleEnvironment)                 │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │ depends on
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│                (ECMABasic.Infrastructure)                   │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ConsoleEnvironment                                    │  │
│  │  • Implements IEnvironment                           │  │
│  │  • Console I/O                                       │  │
│  │  • Delegates to Application layer                    │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │ depends on
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                          │
│                 (ECMABasic.Application)                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ EnvironmentBase (creates per-environment services)   │  │
│  │  • IntrinsicRegistry (per environment)              │  │
│  │  • BasicRandomNumberGenerator (per environment)     │  │
│  │  • MinimalBasicConfiguration (injected or fallback) │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Interpreter & Parsers                                │  │
│  │  • StatementParsers (instantiated in constructor)   │  │
│  │  • ExpressionParsers (static helpers)               │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │ depends on
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│                   (ECMABasic.Domain)                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Interfaces (no implementations, DI-free)             │  │
│  │  • IEnvironment                                      │  │
│  │  • IIntrinsicRegistry                               │  │
│  │  • IRandomNumberGenerator                           │  │
│  │  • IBasicConfiguration                              │  │
│  │  • IExpression, IStatement (pure behavior)         │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Service Lifetimes

### Singleton (One instance per application)
- `IBasicConfiguration` - Configuration is immutable after load
- `Interpreter` - Stateless parser registry
- `IEnvironment` - One environment per application session

### Per-Environment (One instance per environment)
- `IIntrinsicRegistry` - Each environment has independent function registry
- `IRandomNumberGenerator` - Each environment has independent RNG state

### Transient (Not used)
- Parsers and expressions are instantiated as needed, not through DI

## Environment-Scoped Services

Key insight: Each `IEnvironment` instance creates its own instances of:

```csharp
public abstract class EnvironmentBase : IEnvironment
{
    public EnvironmentBase(...)
    {
        // Per-environment instances
        Intrinsics = new IntrinsicRegistry();        // Independent function registry
        Random = new BasicRandomNumberGenerator();    // Independent RNG (seed: 42)
        
        // Injected or fallback
        Configuration = config ?? MinimalBasicConfiguration.Instance;
    }
}
```

**Why per-environment?**
1. **Test Isolation**: Tests don't share state
2. **ECMA-55 Conformance**: Each program run needs independent, repeatable RNG
3. **Future Dialects**: Different environments can have different function sets

## Registration Pattern

### In ECMABasic55/Program.cs

```csharp
private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
{
    // Configuration: Injected from IConfiguration
    services.AddSingleton<IBasicConfiguration>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        return new MinimalBasicConfiguration(configuration);
    });

    // Interpreter: Singleton, stateless parser
    services.AddSingleton<Interpreter, RuntimeInterpreter>();

    // Environment: Singleton for the session, creates per-environment services
    services.AddSingleton<IEnvironment>(sp =>
    {
        var interpreter = (RuntimeInterpreter)sp.GetRequiredService<Interpreter>();
        var env = new ConsoleEnvironment(interpreter);
        InjectIntrinsics(env); // Add non-ECMA-55 extensions (ASC, MID$, POS)
        return env;
    });
}
```

## Function Delegate Pattern

Intrinsic functions receive `IEnvironment` at execution time:

```csharp
// Registration (parse time)
Register("RND", [ExpressionType.Number], (env, args) => 
    env.Random.Next(Convert.ToInt32(args[0])));

// Execution (runtime)
public object Evaluate(IEnvironment env)
{
    // Function delegate receives environment
    return Function(env, Arguments.Select(x => x.Evaluate(env)).ToList());
}
```

**Why pass environment to functions?**
- Functions need access to environment services (Random, Configuration)
- Enables per-environment behavior (different RNG seeds, configs)
- Clean separation: registration (parse time) vs execution (runtime)

## Static vs DI Decision Matrix

| Pattern | Use When | Example |
|---------|----------|---------|
| **DI Singleton** | Service has configuration/state, needed app-wide | `IBasicConfiguration` |
| **DI Per-Environment** | Service needs isolation between environments | `IRandomNumberGenerator` |
| **Static Helpers** | Pure functions, no state, high performance | `StatementParser.ParseExpression()` |
| **Direct Instantiation** | Simple value objects, short-lived | `Token`, `ProgramLine` |

## Testing Patterns

### Test with Custom Configuration

```csharp
[Fact]
public void CustomMaxStringLength_IsRespected()
{
    // Arrange
    var configData = new Dictionary<string, string>
    {
        ["maxStringLength"] = "5"
    };
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();
    var customConfig = new MinimalBasicConfiguration(config);
    
    // Act
    var env = new TestEnvironment(config: customConfig);
    
    // Assert
    Assert.Equal(5, env.Configuration.MaxStringLength);
}
```

### Test with Seeded RNG

```csharp
[Fact]
public void RND_WithFixedSeed_ProducesRepeatableSequence()
{
    // Arrange
    var env1 = new TestEnvironment();
    var env2 = new TestEnvironment();
    
    env1.Random.Reseed(12345);
    env2.Random.Reseed(12345); // Same seed
    
    // Act
    var sequence1 = Enumerable.Range(0, 10).Select(_ => env1.Random.NextDouble()).ToList();
    var sequence2 = Enumerable.Range(0, 10).Select(_ => env2.Random.NextDouble()).ToList();
    
    // Assert
    Assert.Equal(sequence1, sequence2); // Same seed = same sequence
}
```

### Test with Isolated Environments

```csharp
[Fact]
public void MultipleEnvironments_HaveIndependentState()
{
    // Arrange
    var env1 = new TestEnvironment();
    var env2 = new TestEnvironment();
    
    // Act
    env1.Random.NextDouble(); // Advance env1's RNG
    var value1 = env1.Random.NextDouble();
    var value2 = env2.Random.NextDouble(); // env2's RNG is independent
    
    // Assert
    Assert.NotEqual(value1, value2); // Different states
}
```

## Migration History

### Issue #30: FunctionFactory → IntrinsicRegistry
- **Before**: Global singleton `FunctionFactory.Instance`
- **After**: Per-environment `env.Intrinsics`
- **Benefit**: Test isolation, future dialect support

### Issue #41: RandomFactory → IRandomNumberGenerator
- **Before**: Global singleton `RandomFactory.Instance`
- **After**: Per-environment `env.Random`
- **Benefit**: ECMA-55 conformance (repeatable RND), test isolation

### Issue #42: MinimalBasicConfiguration Injection
- **Before**: Static singleton, hardcoded file path
- **After**: Injected from `IConfiguration`, backward-compatible fallback
- **Benefit**: Testability, custom configs

## Anti-Patterns Avoided

❌ **Parser DI Registration**: Would require injecting parsers into Interpreter, adding complexity for no benefit. Parsers are simple, stateless, and work well as direct instantiation.

❌ **Static Helper Conversion**: StatementParser helpers like `ParseExpression()` are pure functions. Converting to DI would require injecting an `IExpressionParserService` into every parser - major overhead, zero benefit.

❌ **Over-DI**: Not everything needs dependency injection. Use DI where it solves real problems (state management, testability, configuration), not everywhere.

## See Also

- Research: `.claude/research/dependency-injection-expansion.md` - Full analysis and decision rationale
- Research: `.claude/research/FunctionFactory-architecture-analysis.md` - IntrinsicRegistry refactor details
- CLAUDE.md - DI guidelines for developers
