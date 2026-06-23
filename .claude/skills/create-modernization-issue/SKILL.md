---
name: create-modernization-issue
description: Create a GitHub issue for a modernization task with proper template
user-invocable: true
argument-hint: <title>
---

# Create Modernization Issue Skill

Creates a properly formatted GitHub issue for modernization tasks following the project's spec-driven development process.

## Usage

```bash
/create-modernization-issue "Fix nullable warnings in CharacterReader"
/create-modernization-issue "Convert to file-scoped namespaces"
```

## What It Does

1. Takes the issue title from arguments
2. Loads the appropriate template
3. Guides you through filling in:
   - Description
   - Current state
   - Target state
   - Files affected
   - Acceptance criteria
4. Formats as GitHub issue markdown
5. Provides the issue content to copy to GitHub

## Template Categories

Choose from:
- **Nullable Reference Types** - CS86xx errors
- **Code Style** - EditorConfig violations
- **Modernization** - C# pattern updates
- **Custom** - Blank template

## Output

I'll provide formatted markdown ready to paste into a new GitHub issue:

```markdown
---
name: Modernization Task
labels: enhancement, modernization
---

## Description
[Your description]

## Current State
[Current code pattern]

## Target State
[Desired modern C# pattern]

## Acceptance Criteria
- [ ] Code compiles without warnings
- [ ] All existing tests pass
- [ ] Test coverage remains ≥ 80%
- [ ] Code follows .editorconfig rules
- [ ] Modern C# patterns applied consistently

## Files Affected
- [ ] src/ECMABasic.Core/CharacterReader.cs
- [ ] ...

## Related Issues
#1 (if applicable)
```

## Example

```
User: /create-modernization-issue "Fix nullable warnings in CharacterReader"

I'll create a GitHub issue for fixing nullable reference type warnings in CharacterReader.cs

[Generates formatted issue with nullable-specific acceptance criteria...]

📋 Copy this content and create a new issue at:
https://github.com/treytomes/ecma_basic/issues/new
```
