# One Type Per File Rule

## Principle

**Each source file must contain exactly ONE top-level type definition.**

## Applies To

- Classes
- Structs
- Records (record, record struct, record class)
- Enums
- Interfaces
- Delegates

## What This Means

### ✅ Correct - One Type Per File

**File: CommandLineProps.cs**
```csharp
namespace ECMABasic55;

internal sealed class CommandLineProps
{
    public string? FilePath { get; init; }
    public string ConfigFile { get; init; } = "appsettings.yaml";
    public bool Debug { get; init; }
}
```

**File: RuntimeConfiguration.cs**
```csharp
namespace ECMABasic55;

public class RuntimeConfiguration
{
    public string Preamble { get; init; } = string.Empty;
}
```

### ❌ Incorrect - Multiple Types in One File

**File: Models.cs** (WRONG)
```csharp
namespace ECMABasic55;

internal sealed class CommandLineProps { }

public class RuntimeConfiguration { }

internal record Settings { }
```

## File Naming Convention

The file name MUST match the primary type name:

- Type: `CommandLineProps` → File: `CommandLineProps.cs`
- Type: `RuntimeConfiguration` → File: `RuntimeConfiguration.cs`
- Type: `IEnvironment` → File: `IEnvironment.cs`

## Exceptions

### Nested Types (Allowed)

Nested types (types declared inside another type) are allowed:

```csharp
// File: OuterClass.cs
public class OuterClass
{
    // Nested type - OK
    private class InnerHelper
    {
        // ...
    }
    
    // Nested enum - OK
    public enum Status
    {
        Active,
        Inactive
    }
}
```

### Helper Types in Same Namespace (Not Allowed)

Even if types are closely related, they must be in separate files:

**Wrong:**
```csharp
// File: Parser.cs
public class Parser { }
public class ParserException : Exception { }  // WRONG - separate file needed
```

**Correct:**
```csharp
// File: Parser.cs
public class Parser { }

// File: ParserException.cs
public class ParserException : Exception { }
```

## Benefits

- **Discoverability**: Easy to find type definitions (1:1 file-to-type mapping)
- **Navigation**: IDE "Go to Definition" always goes to dedicated file
- **Merge Conflicts**: Reduced conflicts when multiple people work on related types
- **Clarity**: File boundaries enforce type boundaries
- **Maintainability**: Smaller files are easier to understand and modify

## When Refactoring

If you find multiple types in a single file:

1. **Create new files** for each additional type
2. **Move type definitions** to their respective files
3. **Verify build** succeeds after extraction
4. **Delete old file** if now empty

## Implementation in This Project

When creating or refactoring code:

1. **Before writing a new type**: Create a dedicated file first
2. **When you see multiple types**: Extract them immediately
3. **File name = Type name**: Always match the primary type
4. **Check your work**: Ensure each `.cs` file has exactly one top-level type

## Enforcement

This rule is enforced via:
- Code review
- Claude Code adherence to this rule document
- Manual checks during refactoring

Consider adding an automated analyzer if this rule is frequently violated.

## Examples from This Project

### Before (Multiple Types)
```csharp
// File: Program.cs
internal sealed class CommandLineProps { }
public static class Program { }
```

### After (One Type Per File)
```csharp
// File: CommandLineProps.cs
internal sealed class CommandLineProps { }

// File: Program.cs
public static class Program { }
```
