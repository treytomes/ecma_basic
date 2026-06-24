# Dependency Injection Expansion Research

**Date**: 2026-06-24  
**Issue**: #33  
**Status**: Research Complete  
**Related**: Issue #30 (✅ Complete - FunctionFactory refactored to environment-scoped IntrinsicRegistry)

## Executive Summary

**Recommendation**: **Selective DI expansion** - Convert specific pain points while preserving simplicity where static patterns work well.

**Priority Conversions**:
1. ✅ **FunctionFactory** → **IntrinsicRegistry** (COMPLETE - Issue #30)
2. 🔴 **RandomFactory** → **IRandomNumberGenerator** service (HIGH - blocks ECMA-55 conformance Issue #35)
3. 🟡 **MinimalBasicConfiguration** → Injected `IBasicConfiguration` (MEDIUM - testability gain)
4. 🟢 **StatementParser static methods** → Keep as-is (LOW - working well, no pain points)

**Do NOT convert**: Parser instantiation in Interpreter constructor (adds complexity, minimal benefit)

---

## Current State Analysis

### ✅ Where DI Works Well (Post Issue #30)

**ECMABasic55 (Presentation Layer)**:
- `IHost` pattern with full service registration
- `RuntimeConfiguration` via `IOptions<T>`
- `IEnvironment` and `Interpreter` injected
- Command-line via `CommandLineProps`

**ECMABasic.Application**:
- `IIntrinsicRegistry` per environment (Issue #30 ✅)
- `IEnvironment` abstraction
- `IBasicConfiguration` interface exists (implementation is singleton)

**ECMABasic.Domain**:
- Pure interfaces, no DI needed ✅

### ❌ Remaining Singletons

#### 1. RandomFactory

**Location**: `ECMABasic.Application/RandomFactory.cs`

**Current Usage**:
```csharp
public static RandomFactory Instance { get; }
private Random _random = new();

public void Reseed(int seed) { _random = new Random(seed); }
public int Next(int maxValue) { return _random.Next(maxValue); }
public double NextDouble() { return _random.NextDouble(); }
```

**Used By**:
- `IntrinsicRegistry` - RND function implementation
- Likely other random number needs

**Problems**:
- **Blocks ECMA-55 conformance**: Issue #35 requires RND repeatability
  - ECMA55-FUN-008: "Without RANDOMIZE, same program = same sequence"
  - ECMA55-RND-001: "RANDOMIZE generates unpredictable seed"
- **Not testable**: Can't seed for deterministic tests
- **Global mutable state**: All environments share same RNG
- **Thread-safety concerns**: Shared Random instance

**ECMA-55 Requirements**:
1. Default seed must be **fixed** for repeatability
2. RANDOMIZE statement must **re-seed** with time-based value
3. Each environment should have **independent** RNG state

#### 2. MinimalBasicConfiguration

**Location**: `ECMABasic.Application/Configuration/MinimalBasicConfiguration.cs`

**Current Usage**:
```csharp
public static IBasicConfiguration Instance { get; } = new MinimalBasicConfiguration();

private MinimalBasicConfiguration()
{
    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .Build();
    
    MaxLineLength = GetValueOrDefault(config, "maxLineLength", 72);
    // ... load other settings
}
```

**Used By**:
- `Interpreter` constructor: `config ?? MinimalBasicConfiguration.Instance`
- `EnvironmentBase` constructor: Same fallback pattern
- Tests (implicitly)

**Problems**:
- **Not testable with different configs**: Can't test with custom MaxStringLength, etc.
- **Hardcoded file path**: `appsettings.json` in working directory
- **Single configuration for all environments**: Can't have dialect-specific configs
- **Load-time file I/O**: Static constructor reads file (slow startup)

**Why It Exists**:
- Convenient fallback when no config provided
- Avoids null checks everywhere
- Simple to use: `MinimalBasicConfiguration.Instance`

### 🟢 Static Patterns That Work Well

#### StatementParser Static Helper Methods

**Location**: `ECMABasic.Application/StatementParser.cs`

**Pattern**:
```csharp
protected static IExpression? ParseExpression(ComplexTokenReader reader, ...)
protected static IExpression? ParseNumericExpression(ComplexTokenReader reader, ...)
protected static VariableExpression? ParseVariableExpression(ComplexTokenReader reader)
```

**Why Keep Static**:
- **Pure functions**: No state, no side effects
- **Performance**: No object allocation overhead
- **Simplicity**: Easy to call from any parser
- **No dependencies**: Just operate on reader and return result
- **No pain points**: Works well, not causing issues

**Alternative (if we converted to DI)**:
```csharp
// Would need to inject IExpressionParserService into every parser
public class LetStatementParser : StatementParser
{
    private readonly IExpressionParserService _expressionParser;
    
    public LetStatementParser(IExpressionParserService expressionParser)
    {
        _expressionParser = expressionParser;
    }
    
    public override IStatement? Parse(...)
    {
        var expr = _expressionParser.ParseExpression(reader, lineNumber, throwOnError);
        // ...
    }
}
```

**Verdict**: ❌ **Do NOT convert** - Adds complexity, no benefit

---

## Detailed Analysis: What to Convert

### 🔴 PRIORITY 1: RandomFactory → IRandomNumberGenerator

**Justification**: BLOCKS ECMA-55 conformance (Issues #35, #36)

#### Problem Statement

ECMA-55 requires:
1. **Deterministic RND without RANDOMIZE**: Same seed → same sequence
2. **Per-environment RNG state**: Each environment independent
3. **RANDOMIZE support**: Re-seed with time-based value

Current singleton prevents all three.

#### Proposed Solution

**Step 1**: Create `IRandomNumberGenerator` interface in Domain

```csharp
namespace ECMABasic.Domain;

/// <summary>
/// Random number generator for BASIC RND function.
/// ECMA-55 requires repeatability (same seed = same sequence).
/// </summary>
public interface IRandomNumberGenerator
{
    /// <summary>
    /// Returns a random number in range [0, 1) per ECMA-55.
    /// </summary>
    double NextDouble();
    
    /// <summary>
    /// Returns a random integer in range [0, maxValue).
    /// </summary>
    int Next(int maxValue);
    
    /// <summary>
    /// Re-seeds the RNG (for RANDOMIZE statement).
    /// </summary>
    void Reseed(int seed);
}
```

**Step 2**: Implement in Application layer

```csharp
namespace ECMABasic.Application;

/// <summary>
/// ECMA-55 compliant random number generator.
/// Uses fixed seed (42) for repeatability per ECMA55-FUN-008.
/// </summary>
public class BasicRandomNumberGenerator : IRandomNumberGenerator
{
    private Random _random;
    private const int DEFAULT_SEED = 42; // Fixed for repeatability
    
    public BasicRandomNumberGenerator()
    {
        _random = new Random(DEFAULT_SEED);
    }
    
    public double NextDouble() => _random.NextDouble();
    
    public int Next(int maxValue) => _random.Next(maxValue);
    
    public void Reseed(int seed)
    {
        _random = new Random(seed);
    }
}
```

**Step 3**: Add to IEnvironment

```csharp
public interface IEnvironment : IErrorReporter
{
    IBasicConfiguration Configuration { get; }
    IIntrinsicRegistry Intrinsics { get; }
    IRandomNumberGenerator Random { get; } // NEW
    // ... existing members
}
```

**Step 4**: Update EnvironmentBase

```csharp
public abstract class EnvironmentBase : IEnvironment
{
    public EnvironmentBase(...)
    {
        // ...
        Intrinsics = new IntrinsicRegistry();
        Random = new BasicRandomNumberGenerator(); // NEW
    }
    
    public IRandomNumberGenerator Random { get; }
}
```

**Step 5**: Update IntrinsicRegistry

```csharp
// In RegisterBuiltInIntrinsics():
// OLD:
Register("RND", [ExpressionType.Number], args => RandomFactory.Instance.Next(...));

// NEW (Issue #35 will change signature to parameterless):
Register("RND", [], args => /* get from environment at runtime */);
```

**Problem**: RND function needs access to environment at **runtime**, not parse time.

**Solution**: Store environment reference in `FunctionExpression`:

```csharp
// FunctionExpression already has access to IEnvironment in Evaluate():
public object Evaluate(IEnvironment env)
{
    return Function(Arguments.Select(x => x.Evaluate(env)).ToList());
}

// So RND function can access env.Random:
Register("RND", [], args => env => env.Random.NextDouble());
```

Wait, that won't work - the delegate signature is `Func<List<object>, object>`, not `Func<IEnvironment, object>`.

**Better Solution**: Pass environment to function via closure or change FunctionDefinition signature.

Actually, looking at how FunctionExpression works:

```csharp
// FunctionExpression.cs line 39-42:
public object Evaluate(IEnvironment env)
{
    return Function(Arguments.Select(x => x.Evaluate(env)).ToList());
}
```

The function delegate is called with just the argument values. We need access to `env` inside the delegate.

**Option A**: Change `Func<List<object>, object>` to `Func<IEnvironment, List<object>, object>`

```csharp
public interface IIntrinsicRegistry
{
    void Register(string name, IEnumerable<ExpressionType> args, 
                  Func<IEnvironment, List<object>, object> fn); // Add IEnvironment param
}

// Usage:
Register("RND", [], (env, args) => env.Random.NextDouble());
```

**Option B**: Store environment in function at registration time via closure

```csharp
// In EnvironmentBase constructor:
private void RegisterBuiltInIntrinsics()
{
    var env = this; // Capture environment
    Intrinsics.Register("RND", [], args => env.Random.NextDouble());
}
```

**Verdict**: **Option A is cleaner** - Makes environment dependency explicit

#### Migration Steps

1. Change `IIntrinsicRegistry.Register` signature to include `IEnvironment` parameter
2. Update `FunctionExpression.Evaluate` to pass `env` to function delegate
3. Create `IRandomNumberGenerator` interface
4. Implement `BasicRandomNumberGenerator`
5. Add `Random` property to `IEnvironment`
6. Update `IntrinsicRegistry` to use `env.Random` instead of `RandomFactory.Instance`
7. Remove `RandomFactory` singleton
8. Update Issue #35 to use new RNG

#### Breaking Changes

- `IIntrinsicRegistry.Register` signature changes
- `FunctionDefinition` constructor signature changes
- Removes `RandomFactory.Instance` (internal API only)

#### Test Impact

- Existing tests continue to work (each environment gets own RNG)
- NEW capability: Can test with seeded RNG for deterministic results
- RANDOMIZE tests can verify independence between environments

#### Estimated Effort

**4-6 hours**

---

### 🟡 PRIORITY 2: MinimalBasicConfiguration → Injected IBasicConfiguration

**Justification**: Improves testability, enables dialect-specific configs

#### Problem Statement

Current singleton:
- Hard to test with different configurations
- Can't have multiple configs (e.g., ECMA-55 vs ECMA-116 dialects)
- Loads from hardcoded file path
- Not DI-friendly

#### Proposed Solution

**Option A**: Inject via constructor (breaking change)

```csharp
// OLD:
public class Interpreter
{
    public Interpreter(IBasicConfiguration? config = null)
    {
        _config = config ?? MinimalBasicConfiguration.Instance;
    }
}

// NEW:
public class Interpreter
{
    public Interpreter(IBasicConfiguration config) // Required, no default
    {
        _config = config;
    }
}
```

**Problem**: Breaks all existing `new Interpreter()` calls

**Option B**: Keep optional parameter, but inject where possible

```csharp
// Keep backward compatibility:
public Interpreter(IBasicConfiguration? config = null)
{
    _config = config ?? MinimalBasicConfiguration.Instance;
}

// But register in DI container:
services.AddSingleton<IBasicConfiguration>(sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MinimalBasicConfiguration(config); // Pass IConfiguration
});

// Then inject:
public RuntimeInterpreter(IBasicConfiguration config)
    : base(config) // Pass to Interpreter
{
}
```

**Option C**: Factory pattern

```csharp
public interface IBasicConfigurationFactory
{
    IBasicConfiguration Create();
}

public class MinimalBasicConfigurationFactory : IBasicConfigurationFactory
{
    private readonly IConfiguration _config;
    
    public MinimalBasicConfigurationFactory(IConfiguration config)
    {
        _config = config;
    }
    
    public IBasicConfiguration Create()
    {
        return new MinimalBasicConfiguration(_config);
    }
}
```

**Verdict**: **Option B** - Non-breaking, enables DI where needed, keeps fallback

#### Migration Steps

1. Update `MinimalBasicConfiguration` to accept `IConfiguration` in constructor
2. Keep static `Instance` for backward compatibility (loads from default file)
3. Register in DI container: `services.AddSingleton<IBasicConfiguration>(...)`
4. Update `EnvironmentBase` and `Interpreter` to prefer injected config
5. Tests can now create custom configs easily

#### Breaking Changes

None - keeps backward compatibility via `Instance` fallback

#### Test Impact

- Existing tests continue to work
- NEW capability: Tests can inject custom configs
- Can test edge cases (MaxStringLength = 1, MaxLineLength = 10, etc.)

#### Estimated Effort

**2-3 hours**

---

## Cost-Benefit Analysis

### RandomFactory Conversion

| Aspect | Cost | Benefit |
|--------|------|---------|
| **Effort** | 4-6 hours | - |
| **Breaking Changes** | Internal API only | Low risk |
| **Testability** | - | ⭐⭐⭐⭐⭐ Huge gain - seeded RNG |
| **ECMA-55 Conformance** | - | ⭐⭐⭐⭐⭐ Blocks Issue #35, #36 |
| **Per-Environment State** | - | ⭐⭐⭐⭐⭐ Each env independent |
| **Complexity** | Signature change | ⭐⭐⭐ Manageable |

**Verdict**: ✅ **HIGH PRIORITY** - Blocks ECMA-55 conformance, huge testability gain

### MinimalBasicConfiguration Conversion

| Aspect | Cost | Benefit |
|--------|------|---------|
| **Effort** | 2-3 hours | - |
| **Breaking Changes** | None (backward compat) | Zero risk |
| **Testability** | - | ⭐⭐⭐⭐ Good gain |
| **Complexity** | Low | ⭐⭐⭐⭐⭐ Simple |
| **Dialect Support** | - | ⭐⭐⭐ Future-proof |

**Verdict**: ✅ **MEDIUM PRIORITY** - Low risk, good gain

### StatementParser Static Methods

| Aspect | Cost | Benefit |
|--------|------|---------|
| **Effort** | 8-12 hours | - |
| **Breaking Changes** | Major refactor | High risk |
| **Testability** | - | ⭐ Minimal (already testable) |
| **Complexity** | High (inject into 20+ parsers) | ⭐ Adds indirection |
| **Current Pain** | None | ⭐ No problems to solve |

**Verdict**: ❌ **DO NOT CONVERT** - High cost, minimal benefit

---

## Recommendation Summary

### ✅ Convert to DI

1. **RandomFactory → IRandomNumberGenerator** (HIGH - Issue #35, #36 blocker)
   - Create `IRandomNumberGenerator` interface
   - Add to `IEnvironment.Random` property
   - Update function signature to pass environment
   - Remove singleton

2. **MinimalBasicConfiguration injection** (MEDIUM - testability)
   - Keep backward compat with `Instance` fallback
   - Register in DI for injection where possible
   - Accept `IConfiguration` in constructor

### ❌ Keep As-Is

1. **StatementParser static methods**
   - Pure functions, no state
   - No pain points
   - High conversion cost, low benefit

2. **Parser instantiation in Interpreter**
   - Current pattern works well
   - DI registration of 20+ parsers adds complexity
   - No testability issues (parsers are simple)

---

## Migration Plan

### Phase 1: RandomFactory (Blocks ECMA-55)

**Issue #41**: Refactor RandomFactory to environment-scoped IRandomNumberGenerator
- Create `IRandomNumberGenerator` interface
- Update function delegate signature to accept `IEnvironment`
- Implement `BasicRandomNumberGenerator` with fixed seed
- Add to `IEnvironment.Random`
- Remove `RandomFactory` singleton
- **Estimated**: 4-6 hours
- **Dependencies**: None
- **Blocks**: Issue #35 (Fix RND), #36 (RANDOMIZE)

### Phase 2: Configuration Injection (Testability)

**Issue #42**: Enable IBasicConfiguration injection while maintaining backward compatibility
- Update `MinimalBasicConfiguration` to accept `IConfiguration`
- Keep `Instance` property for backward compatibility
- Register in DI container
- Update tests to use custom configs
- **Estimated**: 2-3 hours
- **Dependencies**: None
- **Benefits**: Test isolation, dialect support

### Phase 3: Documentation

**Issue #43**: Document DI patterns and best practices
- Update CLAUDE.md with DI guidelines
- Document when to use DI vs. static
- Add examples of testing with DI
- **Estimated**: 1-2 hours

---

## Success Metrics

### Testability Improvements

**Before**:
- ❌ Can't test RND with seeded values
- ❌ Can't test with custom configurations
- ❌ Shared state between test runs

**After**:
- ✅ Can seed RNG for deterministic tests
- ✅ Can inject custom configurations
- ✅ Each test environment isolated

### ECMA-55 Conformance

**Before**:
- ❌ Can't implement ECMA55-FUN-008 (repeatable RND sequence)
- ❌ Can't implement RANDOMIZE per environment

**After**:
- ✅ Fixed seed enables repeatability
- ✅ Per-environment RNG enables RANDOMIZE
- ✅ Unblocks Issues #35, #36

### Code Quality

**Complexity**: Low increase (2 new interfaces, manageable)
**Maintainability**: Improved (clearer dependencies)
**Testability**: Significantly improved
**Performance**: Negligible impact

---

## Conclusion

The FunctionFactory refactor (Issue #30 ✅) demonstrated the value of moving from singletons to environment-scoped services. We should continue this pattern for **RandomFactory** (blocks ECMA-55) and **MinimalBasicConfiguration** (testability), while keeping static patterns that work well (StatementParser helpers).

This selective approach balances Clean Architecture principles with pragmatism - converting pain points while preserving simplicity where static patterns serve us well.

**Recommended Action**: Create Issues #41, #42, #43 and proceed with Phase 1 (RandomFactory) as it blocks ECMA-55 conformance.
