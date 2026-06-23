---
name: create-modernization-issue
description: Create a GitHub issue for a modernization task with proper template
user-invocable: true
argument-hint: <title>
---

# Create Modernization Issue Skill

Creates a GitHub issue directly for modernization tasks following the project's spec-driven development process.

## Usage

```bash
/create-modernization-issue "Fix nullable warnings in CharacterReader"
/create-modernization-issue "Convert to file-scoped namespaces"
```

## What It Does

1. Takes the issue title from arguments
2. Loads the appropriate template based on keywords
3. Customizes with project-specific details (from build audit, etc.)
4. **Creates the issue directly on GitHub** using available tools:
   - GitHub MCP server tools (if available)
   - GitHub CLI (`gh`) as fallback
5. Returns the issue URL and number

## GitHub Integration

This skill uses **GitHub API** via two methods:

### Primary: GitHub MCP Server (Preferred)
When available, uses MCP tools:
- `github_search_repositories` - Find repo
- `github_search_issues` - Check for duplicates
- `github_create_issue` - Create issues
- `github_update_issue` - Update existing issues

**Advantages**: Native API, structured responses, better error handling

### Fallback: GitHub CLI (`gh`)
When MCP unavailable, uses `gh` commands:
- `gh issue list --search "..."` - Search
- `gh issue create --title "..." --body "..."` - Create
- `gh issue edit N --body "..."` - Update

**Advantages**: Always available, no MCP server needed

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

## Implementation

When invoked, I will:

1. **Detect available GitHub integration**:
   - First check: GitHub MCP server tools available?
   - If not: Fall back to `gh` CLI
   - Use whichever is available

2. **Check for existing issues** first:
   ```bash
   gh issue list --repo treytomes/ecma_basic \
     --search "nullable OR reference OR CS86" \
     --state all --limit 20
   ```
   - If similar issue exists and is **open**: Update it instead of creating duplicate
   - If similar issue exists and is **closed**: Reference it and create new one
   - If no similar issues: Proceed with creation

2. **Assess issue size**:
   - **Small** (< 3 hours, < 20 files): Single issue
   - **Medium** (3-8 hours, 20-50 files): Single issue with subtasks
   - **Large** (> 8 hours, > 50 files): Break into multiple issues
   - Example: "Fix 127 nullable errors" → Break into:
     - Issue #1a: Fix nullable errors in parsers (40 errors)
     - Issue #1b: Fix nullable errors in expressions (35 errors)
     - Issue #1c: Fix nullable errors in statements (30 errors)
     - Issue #1d: Fix nullable errors in environment (22 errors)

3. **Detect issue type** from title keywords:
   - "nullable" → Use nullable.md template
   - "file-scoped" or "namespace" → Use namespace template
   - "accessibility" or "modifier" → Use style template
   - Default → Use generic template

4. **Load appropriate template** from `templates/` directory

5. **Customize with project data**:
   - Pull error counts from latest build audit
   - List most affected files
   - Include relevant code examples
   - Set accurate effort estimates
   - Add task breakdown if medium/large

6. **Create issue(s) on GitHub** using `gh issue create`:
   ```bash
   gh issue create \
     --repo treytomes/ecma_basic \
     --title "Issue Title" \
     --body "$(cat issue-body.md)" \
     --label bug,modernization \
     --milestone "v0.3.0"
   ```

7. **Return issue URL(s) and number(s)** for reference

## Example

```
User: /create-modernization-issue "Fix nullable reference type violations"

Me: 📋 Creating GitHub Issue...

    Type detected: Nullable Reference Types
    Template: nullable.md
    Data source: .claude/audits/build-audit-2026-06-23.md
    
    Customizing with:
    - 127 CS86xx errors across 40 files
    - Top files: Interpreter.cs (21), IEnvironment.cs (15)
    - Effort estimate: 8-12 hours
    
    Creating issue on GitHub...
    
    ✅ Issue created successfully!
    
    Issue #1: Fix nullable reference type violations
    URL: https://github.com/treytomes/ecma_basic/issues/1
    
    Labels: bug, modernization
    Milestone: v0.3.0
    
    Ready to start work on this issue!
```
