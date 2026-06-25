---
name: modernization
description: Modern .NET and C# coding standards for this project
---

# Modernization Guidelines

## .NET Version
- Target .NET 10 for all projects
- Use latest C# language features (C# 13)
- All projects inherit from `Directory.Build.props` in src/ folder

## Code Style Standards

### Nullable Reference Types
- Enable nullable reference types in all projects: `<Nullable>enable</Nullable>`
- Explicitly annotate nullable vs non-nullable types
- Use null-coalescing and null-conditional operators where appropriate

### Modern C# Patterns
- **File-scoped namespaces**: Use `namespace ECMABasic.Application;` instead of block syntax
- **Target-typed new**: Use `new()` when type is obvious from context
- **var keyword**: Always use `var` for local variable declarations
- **Pattern matching**: Prefer pattern matching over traditional type checks
- **Primary constructors**: Consider for classes with simple initialization (C# 12+)
- **Collection expressions**: Use `[item1, item2]` syntax (C# 12+)
- **String interpolation**: Use `$"{value}"` over `string.Format` or concatenation

### Thread-Local State Management

When you need per-thread state in static contexts (e.g., parsing environment):

```csharp
// Thread-local static field
[ThreadStatic]
private static IEnvironment? _currentParsingEnvironment;

// Public read-only property
public static IEnvironment? CurrentParsingEnvironment => _currentParsingEnvironment;

// Protected setter for derived classes
protected static void SetCurrentParsingEnvironment(IEnvironment? env)
{
    _currentParsingEnvironment = env;
}
```

**Use Cases**:
- Parser contexts that need global access without passing through every method
- Per-thread execution state in multi-threaded environments
- Avoiding parameter pollution in deep call stacks

**Pattern Example** (from Interpreter.cs):
```csharp
public class Interpreter
{
    [ThreadStatic]
    private static IEnvironment? _currentParsingEnvironment;
    
    public static IEnvironment? CurrentParsingEnvironment => _currentParsingEnvironment;
    
    protected static void SetCurrentParsingEnvironment(IEnvironment? env)
    {
        _currentParsingEnvironment = env;
    }
    
    private bool InterpretProgram(IEnvironment env)
    {
        SetCurrentParsingEnvironment(env);
        try
        {
            // Parsing code can now access CurrentParsingEnvironment
            // without passing env through every method
        }
        finally
        {
            SetCurrentParsingEnvironment(null); // Always clean up
        }
    }
}
```

**Important**:
- Always use `finally` block to clear thread-local state
- Document why thread-local is needed (avoid overuse)
- Consider alternatives (dependency injection, explicit parameters) first
- Thread-local state doesn't cross thread boundaries

### Code Organization
- One class per file
- File name matches primary type name
- Keep classes focused and single-responsibility

### Naming Conventions
- Public members: PascalCase
- Private fields: _camelCase with underscore prefix
- Local variables and parameters: camelCase
- Constants: PascalCase
- Interfaces: IPascalCase

### Documentation
- XML comments on all public APIs
- Keep comments focused on WHY, not WHAT
- Update docs when refactoring

## Testing Standards

### Coverage Requirements
- **Minimum 80% code coverage** for all projects
- Test coverage measured via `dotnet test --collect:"XPlat Code Coverage"`
- Focus on meaningful tests, not just coverage metrics

### Test Structure
- One test class per production class
- Test method naming: `MethodName_Scenario_ExpectedResult`
- Use Arrange-Act-Assert pattern
- Keep tests isolated and independent

### Test Categories
- Unit tests: Test individual components in isolation
- Integration tests: Test component interactions
- Sample tests: Validate ECMA-55 standard compliance

## Spec-Driven Development

### Process
1. **Specification First**: Define behavior through tests before implementation
2. **Red-Green-Refactor**: Write failing test → make it pass → improve code
3. **Living Documentation**: Tests serve as executable specifications

### Test Organization
- Group related tests by feature/specification area
- Use descriptive test names that read like specifications
- Keep test data in Resources/ for data-driven tests

## Clean Architecture Principles

### Current Architecture
This project follows a layered architecture suitable for its interpreter domain:
- **Core**: Domain logic and abstractions (no external dependencies)
- **55**: Application layer (console REPL, extends Core)
- **Test**: Validation of specifications

### Dependency Rules
- Core has no dependencies on other projects
- ECMABasic55 depends only on Core
- Test depends only on what it's testing
- Dependencies point inward (toward Core)

### Abstractions
- Use interfaces for extensibility (`IEnvironment`, `IBasicConfiguration`)
- Parser pattern enables pluggable statement types
- Configuration via dependency injection

## Code Quality Checks

### Compiler Configuration
- **Warnings as Errors**: Enabled via `Directory.Build.props` - build fails on ANY warning
- **Nullable Reference Types**: Required for all projects
- **Code Style Enforcement**: EditorConfig rules enforced at build time
- **XML Documentation**: Generated for all projects (warning 1591 suppressed for private members)

### Before Committing
- All tests pass: `dotnet test`
- Code builds without warnings (fails if warnings exist)
- Coverage meets 80% threshold
- No nullable warnings
- XML docs complete for public APIs

### Adding New Projects
New projects automatically inherit these settings from `src/Directory.Build.props`:
- TreatWarningsAsErrors
- Nullable reference types
- Latest C# language version
- EnforceCodeStyleInBuild

No need to manually configure each new .csproj file.
