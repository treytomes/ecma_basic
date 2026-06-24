# FunctionFactory Architecture Analysis

**Date**: 2026-06-24  
**Issue**: #30  
**Status**: Research Complete

## Executive Summary

**Recommendation**: **Option C - Environment-Scoped Registry with DI**

- Rename `FunctionFactory` → `IntrinsicRegistry`
- Move from global singleton to environment-scoped instance
- Inject via `IEnvironment.Intrinsics` property
- Register built-in functions during environment construction
- Keep implementation in Application layer (uses .NET APIs)

**Naming Rationale**: "Intrinsic" is more precise than "Function" - these are language intrinsics (ECMA-55 Section 7) distinct from user-defined functions (DEF FN, Section 8).

**Rationale**: Best balance of testability, DI compatibility, and architectural clarity. Allows isolated test environments and future dialect support while maintaining simplicity.

---

## Current Architecture Analysis

### Implementation Review

**Location**: `ECMABasic.Application.FunctionFactory`

**Pattern**: Singleton
```csharp
public class FunctionFactory
{
    public static FunctionFactory Instance { get; }
    private FunctionFactory() { /* registers built-ins */ }
}
```

**Usage Points**:
1. `Program.cs` - Adds runtime-specific functions (ASC, MID$, POS) via `InjectIntrinsics()`
2. `NumericExpressionParser.cs` - Queries for numeric functions during parsing
3. `StringExpressionParser.cs` - Queries for string functions during parsing

### Problems with Current Design

1. **Global Mutable State**
   - All environments share the same function registry
   - Tests can't isolate function sets
   - User-defined functions in one session affect all sessions

2. **Not DI-Friendly**
   - Static singleton pattern doesn't work with dependency injection
   - Can't mock or replace for testing
   - Can't configure different function sets per environment

3. **Naming Confusion**
   - "Factory" implies object creation
   - Actually a registry/repository pattern

4. **Mixed Responsibilities**
   - Core BASIC functions (ABS, COS, INT, RND, SGN, SIN, TAN) in Application
   - Extended functions (ASC, MID$, POS) in Program.cs
   - No clear boundary between "intrinsic" and "extension"

---

## ECMA-55 Specification Review

### Required Functions (ECMA-55 Minimal BASIC)

**Numeric Functions** (Section 7):
- `ABS(x)` - Absolute value
- `ATN(x)` - Arctangent
- `COS(x)` - Cosine
- `EXP(x)` - Exponential (e^x)
- `INT(x)` - Integer part
- `LOG(x)` - Natural logarithm
- `RND(x)` - Random number
- `SGN(x)` - Sign (-1, 0, +1)
- `SIN(x)` - Sine
- `SQR(x)` - Square root
- `TAN(x)` - Tangent

**String Functions** (Section 7):
- `CHR$(x)` - Character from ASCII code
- `LEN(s$)` - String length
- `STR$(x)` - Number to string
- `VAL(s$)` - String to number

**Currently Implemented**:
✅ ABS, COS, INT, RND, SGN, SIN, TAN  
❌ ATN, EXP, LOG, SQR (TODO in FunctionFactory.cs)  
❌ CHR$, LEN, STR$, VAL

### Architectural Insight

**Functions are part of the language specification** (domain knowledge), but their **implementations use platform APIs** (.NET Math class, Random, etc.) which are infrastructure concerns.

This suggests a **separation of interface (Domain) and implementation (Application/Infrastructure)**.

---

## Architecture Options Comparison

### Option A: Injectable Singleton (Minimal Change)

```csharp
// Application layer
public interface IFunctionRegistry
{
    void Register(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn);
    IEnumerable<FunctionDefinition> Get(string name);
}

public class FunctionRegistry : IFunctionRegistry
{
    private readonly List<FunctionDefinition> _functions = new();
    
    public FunctionRegistry()
    {
        RegisterBuiltIns();
    }
    
    private void RegisterBuiltIns()
    {
        Register("ABS", [ExpressionType.Number], args => Math.Abs(...));
        // ... other built-ins
    }
}

// DI registration
services.AddSingleton<IFunctionRegistry, FunctionRegistry>();
```

**Pros**:
- Minimal code changes
- Still globally accessible via DI
- Mockable for testing

**Cons**:
- Still global shared state
- Can't have isolated test environments
- All environments share same functions
- Can't support different BASIC dialects easily

**Verdict**: ❌ Doesn't solve the core problem of global state

---

### Option B: Domain Specification Pattern

```csharp
// Domain layer - pure specification
public interface IIntrinsicFunction
{
    string Name { get; }
    IEnumerable<ExpressionType> ParameterTypes { get; }
    object Execute(List<object> args);
}

// Application layer - implementations
public class AbsFunction : IIntrinsicFunction
{
    public string Name => "ABS";
    public IEnumerable<ExpressionType> ParameterTypes => [ExpressionType.Number];
    public object Execute(List<object> args) => Math.Abs(Convert.ToDouble(args[0]));
}

// Infrastructure layer - registry
public class IntrinsicFunctionRegistry
{
    private readonly Dictionary<string, List<IIntrinsicFunction>> _functions = new();
    
    public IntrinsicFunctionRegistry(IEnumerable<IIntrinsicFunction> functions)
    {
        foreach (var fn in functions)
        {
            // Register each function
        }
    }
}

// DI registration
services.AddSingleton<IIntrinsicFunction, AbsFunction>();
services.AddSingleton<IIntrinsicFunction, CosFunction>();
// ... 20+ more registrations
services.AddSingleton<IntrinsicFunctionRegistry>();
```

**Pros**:
- Clean separation of concerns
- Each function is independently testable
- Functions can have dependencies injected
- Very OOP-friendly

**Cons**:
- **MASSIVE boilerplate** - one class per function (20+ classes)
- Over-engineered for simple mathematical wrappers
- Harder to add user-defined functions dynamically
- More complex registration

**Verdict**: ❌ Too much ceremony for simple function wrappers

---

### Option C: Environment-Scoped Registry (RECOMMENDED)

```csharp
// Domain layer - interface only
public interface IIntrinsicRegistry
{
    void Register(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn);
    IEnumerable<FunctionDefinition> Get(string name);
}

public interface IEnvironment
{
    IIntrinsicRegistry Intrinsics { get; }
    // ... existing members
}

// Application layer - implementation
public class IntrinsicRegistry : IIntrinsicRegistry
{
    private readonly List<FunctionDefinition> _functions = new();
    
    public void Register(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn)
    {
        _functions.Add(new FunctionDefinition(name, args, fn));
    }
    
    public IEnumerable<FunctionDefinition> Get(string name)
    {
        return _functions.Where(f => f.Name == name);
    }
}

// Application layer - environment extension
public abstract class EnvironmentBase : IEnvironment
{
    protected EnvironmentBase(...)
    {
        Intrinsics = new IntrinsicRegistry();
        RegisterBuiltInIntrinsics();
    }
    
    public IIntrinsicRegistry Intrinsics { get; }
    
    private void RegisterBuiltInIntrinsics()
    {
        Intrinsics.Register("ABS", [ExpressionType.Number], args => Math.Abs(...));
        Intrinsics.Register("COS", [ExpressionType.Number], args => Math.Cos(...));
        // ... other ECMA-55 required intrinsics
    }
}

// Usage in parsers
var fndef = environment.Intrinsics.Get(nameToken.Text);

// Usage in Program.cs InjectIntrinsics
env.Intrinsics.Register("ASC", [ExpressionType.String], args => ...);
```

**Pros**:
- ✅ **Environment isolation** - each environment has its own function registry
- ✅ **Testable** - tests can create environments with custom function sets
- ✅ **DI-compatible** - no static singletons
- ✅ **Future-proof** - supports different BASIC dialects per environment
- ✅ **Simple** - minimal code changes
- ✅ **Clear ownership** - functions belong to environments, not global state

**Cons**:
- Requires passing `IEnvironment` to parsers (already done in some places)
- Slightly more memory (each environment has its own registry)
- Built-in functions registered per-environment (minor overhead)

**Verdict**: ✅ **RECOMMENDED** - Best balance of benefits vs complexity

---

## Implementation Plan (Option C)

### Phase 1: Rename and Extract Interface

**File**: `src/ECMABasic.Application/FunctionFactory.cs`
1. Rename `FunctionFactory` → `IntrinsicRegistry`
2. Extract `IIntrinsicRegistry` interface
3. Make constructor public (remove singleton pattern)
4. Keep `RegisterBuiltInIntrinsics()` as internal method

**File**: `src/ECMABasic.Domain/IIntrinsicRegistry.cs`
1. Create interface with `Register()` and `Get()` methods
2. Move to Domain layer (pure specification, no implementation)

### Phase 2: Add to IEnvironment

**File**: `src/ECMABasic.Domain/IEnvironment.cs`
1. Add `IIntrinsicRegistry Intrinsics { get; }` property

**File**: `src/ECMABasic.Application/EnvironmentBase.cs`
1. Create `IntrinsicRegistry` instance in constructor
2. Call `RegisterBuiltInIntrinsics()` during initialization
3. Expose via `Intrinsics` property

### Phase 3: Update Parsers

**File**: `src/ECMABasic.Application/NumericExpressionParser.cs`
1. Change `FunctionFactory.Instance.Get(...)` → `_environment.Intrinsics.Get(...)`
2. Pass environment to parser (if not already available)

**File**: `src/ECMABasic.Application/StringExpressionParser.cs`
1. Same changes as NumericExpressionParser

### Phase 4: Update Program.cs

**File**: `src/ECMABasic55/Program.cs`
1. Change `FunctionFactory.Instance.Define(...)` → `env.Intrinsics.Register(...)`
2. Already have access to `IEnvironment` in `InjectIntrinsics()`

### Phase 5: Remove Singleton

1. Delete static `Instance` property
2. Delete static constructor
3. Update any remaining usages

### Phase 6: Add Missing ECMA-55 Functions

Add to `RegisterBuiltInFunctions()`:
- `ATN` - Math.Atan
- `EXP` - Math.Exp
- `LOG` - Math.Log
- `SQR` - Math.Sqrt
- `CHR$` - Convert.ToChar
- `LEN` - string.Length
- `STR$` - ToString
- `VAL` - Convert.ToDouble

---

## Impact Assessment

### Breaking Changes

**External API** (if any consumers):
- `FunctionFactory.Instance` → `environment.Functions`
- Method rename: `Define()` → `Register()`

**Internal Changes**:
- Parsers need `IEnvironment` reference (likely already have it)
- Tests that directly use `FunctionFactory.Instance` need updates

### Test Changes Required

1. **Existing tests** - minimal impact (environments already created)
2. **New capability** - tests can now register custom functions per test
3. **Isolation** - tests no longer share global function state

### Performance Impact

**Negligible**:
- Function registration happens once per environment creation
- Function lookup is same (dictionary lookup)
- Memory: ~1KB per environment for registry (insignificant)

---

## Alternative Considered: Hybrid Approach

**Idea**: Shared read-only built-in registry + per-environment user-defined registry

```csharp
public static class BuiltInFunctions
{
    private static readonly FunctionRegistry _builtIns = new();
    static BuiltInFunctions() { /* register ECMA-55 functions */ }
    public static IFunctionRegistry Instance => _builtIns;
}

public class EnvironmentFunctionRegistry : IFunctionRegistry
{
    private readonly IFunctionRegistry _builtIns;
    private readonly FunctionRegistry _userDefined = new();
    
    public EnvironmentFunctionRegistry(IFunctionRegistry builtIns)
    {
        _builtIns = builtIns;
    }
    
    public IEnumerable<FunctionDefinition> Get(string name)
    {
        // Check user-defined first, then built-ins
        return _userDefined.Get(name).Concat(_builtIns.Get(name));
    }
}
```

**Pros**:
- Built-ins registered once (performance)
- User-defined functions isolated per environment

**Cons**:
- More complex
- Still has global state (built-ins)
- Doesn't support dialect variations

**Verdict**: ❌ Premature optimization - Option C is simpler and adequate

---

## Recommendation Summary

### Implement Option C: Environment-Scoped Registry

**Key Changes**:
1. Rename `FunctionFactory` → `IntrinsicRegistry`
2. Extract `IIntrinsicRegistry` interface (Domain layer)
3. Remove singleton pattern
4. Add `IIntrinsicRegistry Intrinsics` property to `IEnvironment`
5. Register built-in intrinsics in `EnvironmentBase` constructor
6. Update parsers to use `environment.Intrinsics` instead of singleton
7. Update `Program.cs` to use `env.Intrinsics.Register()`

**Benefits**:
- ✅ DI-compatible (no static singletons)
- ✅ Testable (isolated environments)
- ✅ Future-proof (dialect support)
- ✅ Clear architecture (functions belong to environments)
- ✅ Minimal complexity (simple implementation)

**Estimated Effort**: 3-4 hours
- Rename and refactor: 1 hour
- Update usages: 1 hour
- Add missing ECMA-55 functions: 1 hour
- Testing and verification: 1 hour

---

## Follow-Up Issues

1. **Implementation Issue**: "Refactor FunctionFactory to environment-scoped FunctionRegistry"
   - Implement Option C as described above
   - Estimated: 3-4 hours

2. **Enhancement Issue**: "Add missing ECMA-55 intrinsic functions"
   - Add ATN, EXP, LOG, SQR, CHR$, LEN, STR$, VAL
   - Estimated: 1-2 hours
   - Can be done as part of implementation or separately

3. **Future Consideration**: "Support ECMA-116 function set"
   - Research ECMA-116 additional functions
   - Design dialect-specific function registration
   - Low priority until ECMA-116 support is needed

---

## References

- ECMA-55 Minimal BASIC Standard (1978)
- `FunctionFactory.cs` TODO comment: "This really needs to be part of the environment rather than a global singleton"
- Issue #28: DI implementation already in place
- Issue #33: DI expansion research (broader context)

---

## Conclusion

The current `FunctionFactory` singleton pattern is incompatible with modern DI practices and prevents test isolation. **Option C (Environment-Scoped Registry)** provides the best balance of simplicity, testability, and architectural clarity.

The recommended changes are straightforward, non-breaking (internal APIs), and set the foundation for future enhancements like user-defined functions and dialect support.

**Next Step**: Create implementation issue and proceed with refactoring.

---

## Update: Naming Decision (2026-06-24)

After discussion, confirmed **`IntrinsicRegistry`** is the better name over `FunctionRegistry`:

**Rationale**:
- More precise terminology - these are language intrinsics, not arbitrary functions
- ECMA-55 distinguishes intrinsic functions (Section 7) from user-defined functions (DEF FN, Section 8)
- Matches compiler/interpreter terminology ("intrinsics" or "built-ins")
- Future-proof for DEF FN support (user-defined functions would be separate)

**Final naming**:
- `IIntrinsicRegistry` (interface in Domain layer)
- `IntrinsicRegistry` (implementation in Application layer)  
- `IEnvironment.Intrinsics` (property for access)
- Usage: `env.Intrinsics.Register(...)` and `env.Intrinsics.Get(...)`

All references in this document have been updated to reflect this decision.
