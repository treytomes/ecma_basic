# ECMABasic Configuration Summary

## Build Configuration Hierarchy

### Directory.Build.props (New!)
**Location**: `src/Directory.Build.props`

This file automatically applies to ALL projects in the src/ directory and ensures consistent quality standards:

```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
<Nullable>enable</Nullable>
<LangVersion>latest</LangVersion>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

**Why this matters**:
- ✅ Any new project you create automatically gets these settings
- ✅ No need to remember to configure each .csproj individually
- ✅ Consistent standards across the entire solution
- ✅ Build FAILS on ANY warning (no exceptions)

### .editorconfig
**Location**: Root directory

Defines code style rules that are enforced at build time:
- File-scoped namespaces (warning)
- `var` keyword preference (warning)
- Accessibility modifiers required (error)
- Nullable reference type rules
- Naming conventions

### .csproj Files
Individual projects can still override Directory.Build.props settings if needed, but they inherit the defaults.

## Warnings as Errors

### What It Means
ANY compiler warning causes the build to **FAIL**:
- ❌ CS8625: Null literal to non-nullable type → Build fails
- ❌ IDE0040: Missing accessibility modifier → Build fails
- ❌ CS1591: Missing XML documentation → Build fails (suppressed via NoWarn 1591)

### Why This Is Good
- Forces quality at development time, not review time
- Prevents "fix it later" technical debt
- Makes CI/CD pipeline reliable (no surprise failures)
- Ensures code meets standards before commit

### Current Status
The build currently fails with ~170 errors. This is EXPECTED and CORRECT. These errors must be fixed before the code will compile.

## Code Style Preferences

### var Keyword
**Standard**: Use `var` for ALL local variable declarations

```csharp
// ✅ Correct
var reader = new CharacterReader(stream);
var count = GetCount();
var name = "test";

// ❌ Incorrect (will warn)
CharacterReader reader = new CharacterReader(stream);
int count = GetCount();
string name = "test";
```

**Enforcement**: EditorConfig with `:warning` severity

### File-Scoped Namespaces
**Standard**: Use C# 10 file-scoped namespace syntax

```csharp
// ✅ Correct
namespace ECMABasic.Application;

public class CharacterReader
{
    // ...
}

// ❌ Incorrect (will warn)
namespace ECMABasic.Application
{
    public class CharacterReader
    {
        // ...
    }
}
```

### Nullable Reference Types
**Standard**: Explicit null handling with `?` notation

```csharp
// ✅ Correct
public string? NullableString { get; set; }
public string NonNullString { get; set; } = string.Empty;

public string? TryGetValue()
{
    return condition ? "value" : null;
}

// ❌ Incorrect (will error)
public string NullableString { get; set; } = null; // CS8625
```

## Adding New Projects

When you create a new project:

```bash
dotnet new classlib -n MyNewProject -f net10.0
```

It will AUTOMATICALLY inherit:
- ✅ Warnings as errors
- ✅ Nullable reference types
- ✅ Latest C# language version
- ✅ Code style enforcement
- ✅ Documentation generation

**You don't need to configure anything manually!**

## Overriding Settings

If a project needs to override Directory.Build.props (rare):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <!-- Override: Allow warnings in this specific project -->
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

**⚠️ Warning**: Only override when absolutely necessary. The defaults exist for quality reasons.

## Quality Gates

### Local Development
```bash
# These commands will fail if quality standards not met:
./build.sh    # Fails on warnings
./test.sh     # Fails if coverage < 80%
```

### CI/CD Pipeline
GitHub Actions enforces:
1. ✅ Build succeeds (zero warnings)
2. ✅ All tests pass
3. ✅ Code coverage ≥ 80%
4. ✅ EditorConfig rules followed

**PR cannot be merged** if any gate fails.

## Summary

| Setting | Enforced By | Severity | Can Override? |
|---------|------------|----------|---------------|
| Warnings as Errors | Directory.Build.props | Build Failure | Yes (per-project) |
| Nullable Types | Directory.Build.props | Build Failure | Yes (per-project) |
| Code Style | .editorconfig | Warning/Error | No (project-wide) |
| var Keyword | .editorconfig | Warning | No (project-wide) |
| 80% Coverage | CI/CD + test.sh | Build Failure | No (enforced) |

## Related Files

- `src/Directory.Build.props` - Global MSBuild properties
- `.editorconfig` - Code style rules
- `.claude/rules/modernization.md` - Development standards
- `.claude/settings.json` - Claude Code permissions
- `.github/workflows/ci.yml` - CI/CD pipeline

## Questions?

See [CLAUDE.md](../CLAUDE.md) for full development guidelines.
