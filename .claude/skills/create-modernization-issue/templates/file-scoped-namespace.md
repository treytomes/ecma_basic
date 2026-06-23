## Description
Convert to file-scoped namespace declarations (C# 10 feature)

## Current State
Uses block-scoped namespaces:
```csharp
namespace ECMABasic.Core
{
    public class ClassName
    {
        // ...
    }
}
```

## Target State
Uses file-scoped namespaces:
```csharp
namespace ECMABasic.Core;

public class ClassName
{
    // ...
}
```

## Acceptance Criteria
- [ ] All C# files use file-scoped namespace syntax
- [ ] Indentation reduced by one level throughout file
- [ ] All existing tests pass
- [ ] Test coverage remains ≥ 80%
- [ ] Code follows .editorconfig rules
- [ ] No functional changes to code behavior

## Files Affected
- [ ] All 128 C# source files in src/

## Automation Note
This can be partially automated with:
```bash
dotnet format --include src/ --fix-style warn
```

Or via IDE refactoring: "Convert to file-scoped namespace"

## Testing Strategy
- Run full test suite after conversion
- Visual inspection of a few files to verify proper formatting
- No behavioral tests needed (pure syntax change)

## Related Issues
Part of .NET 10 modernization effort
