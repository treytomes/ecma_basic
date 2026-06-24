# Create Modernization Issue Skill

## Quick Reference

```bash
/create-modernization-issue "Fix nullable reference type violations"
```

## What It Does

1. **Searches for existing issues** to avoid duplicates
2. **Assesses scope** and breaks large issues into smaller ones if needed
3. **Generates customized content** from templates using build audit data
4. **Creates issue(s) on GitHub** automatically with proper labels and milestones
5. **Returns URLs** for created/updated issues

## Features

### Duplicate Prevention ✅
- Searches existing issues before creating
- Updates open issues if similar work already tracked
- References closed issues for context

### Smart Sizing ✅
- **Small** (< 3 hours): Single issue
- **Medium** (3-8 hours): Single issue with subtasks
- **Large** (> 8 hours): Automatically breaks into multiple focused issues

### Template-Driven ✅
- Nullable reference types template
- File-scoped namespaces template
- Code style template
- Generic modernization template

### Data-Driven ✅
- Pulls error counts from latest build audit
- Lists most affected files
- Includes accurate effort estimates
- Provides code pattern examples

## Templates

Located in `templates/` directory:

- `nullable.md` - For CS86xx nullable reference type errors
- `namespace.md` - For file-scoped namespace conversion
- `style.md` - For code style fixes (IDE0xxx)
- `generic.md` - For other modernization tasks

## GitHub Integration

Uses **GitHub API** via two methods (automatically detects which is available):

### Method 1: GitHub MCP Server (Preferred)
When MCP server is active:
- Native GitHub API calls
- Structured responses
- Better error handling
- Richer metadata access

**Setup**:
1. GitHub MCP server enabled in `.mcp.json` (`"disabled": false`)
2. `GITHUB_TOKEN` in `.claude/settings.local.json` or environment
3. Restart Claude Code to load MCP server

**Check if active**: After restart, GitHub MCP tools will be available via ToolSearch

### Method 2: GitHub CLI (Fallback)
When MCP server not available:
- Uses `gh` command-line tool
- Works immediately without restart
- Slightly less structured output

**Setup**:
- GitHub CLI (`gh`) installed and authenticated
- `gh auth status` shows logged in

### Automatic Detection

The skill automatically:
1. Checks if GitHub MCP tools are available
2. If yes: Uses MCP server (preferred)
3. If no: Falls back to `gh` CLI
4. Reports which method is being used

## Environment Setup

**Minimum Requirements** (works now):
- ✅ GitHub CLI (`gh`) installed and authenticated

**Recommended Setup** (better experience):
- ✅ GitHub CLI (`gh`) installed and authenticated
- ✅ `GITHUB_TOKEN` in `.claude/settings.local.json`
- ✅ GitHub MCP server enabled in `.mcp.json`
- ⚠️ **Restart Claude Code** to activate MCP server

## Examples

### Example 1: Creating a New Issue

```bash
User: /create-modernization-issue "Fix nullable reference type violations"

Me: 🔍 Checking for existing issues...
    
    No similar open issues found.
    
    📊 Analyzing scope...
    
    Found: 127 CS86xx errors across 40 files
    Estimated effort: 8-12 hours
    Assessment: LARGE - Breaking into 4 focused issues
    
    📋 Creating issues on GitHub...
    
    ✅ Created Issue #1: Fix nullable errors in parsers (40 errors)
       https://github.com/treytomes/ecma_basic/issues/1
       
    ✅ Created Issue #2: Fix nullable errors in expressions (35 errors)
       https://github.com/treytomes/ecma_basic/issues/2
       
    ✅ Created Issue #3: Fix nullable errors in statements (30 errors)
       https://github.com/treytomes/ecma_basic/issues/3
       
    ✅ Created Issue #4: Fix nullable errors in environment (22 errors)
       https://github.com/treytomes/ecma_basic/issues/4
    
    All issues labeled: bug, modernization
    Milestone: v0.3.0
    
    Ready to start with Issue #1!
```

### Example 2: Updating Existing Issue

```bash
User: /create-modernization-issue "Convert to file-scoped namespaces"

Me: 🔍 Checking for existing issues...
    
    Found similar open issue:
    Issue #3: Convert all files to file-scoped namespaces
    https://github.com/treytomes/ecma_basic/issues/3
    
    📝 Updating Issue #3 with latest data...
    
    Added:
    - Current status: 93 files remaining
    - Progress: 0% complete
    - Updated file list
    - Refreshed effort estimate: 2-3 hours
    
    ✅ Issue #3 updated successfully!
    
    Continue work on existing issue instead of creating duplicate.
```

### Example 3: Small Issue (No Breaking)

```bash
User: /create-modernization-issue "Fix XML documentation warnings"

Me: 🔍 Checking for existing issues...
    
    No similar open issues found.
    
    📊 Analyzing scope...
    
    Found: 10 CS1570 errors in TokenType.cs
    Estimated effort: 30 minutes
    Assessment: SMALL - Creating single issue
    
    📋 Creating issue on GitHub...
    
    ✅ Created Issue #5: Fix XML documentation warnings
       https://github.com/treytomes/ecma_basic/issues/5
       
    Labels: bug, modernization, good-first-issue
    Milestone: v0.3.0
    
    Ready to start!
```

## Issue Breakdown Strategy

When breaking large issues:

### By Component
```
Issue #1a: Fix nullable errors in parsers
Issue #1b: Fix nullable errors in expressions
Issue #1c: Fix nullable errors in statements
Issue #1d: Fix nullable errors in environment
```

### By Error Type
```
Issue #1a: Fix CS8603 (possible null return) - 72 errors
Issue #1b: Fix CS8625 (null literal) - 30 errors
Issue #1c: Fix CS8602/CS8618/CS8604 (other nullable) - 25 errors
```

### By Priority
```
Issue #1a: Fix critical nullable errors (high-traffic files)
Issue #1b: Fix medium-priority nullable errors
Issue #1c: Fix low-priority nullable errors (edge cases)
```

## How It Works Internally

1. **Search Phase**
   ```bash
   gh issue list --search "nullable OR CS86" --state all --limit 20
   ```

2. **Scope Assessment**
   - Read `.claude/audits/build-audit-*.md`
   - Count affected files and errors
   - Calculate effort estimate
   - Determine if breaking needed

3. **Template Loading**
   - Match keywords in title to template
   - Load appropriate template from `templates/`
   - Merge with project-specific data

4. **Issue Creation**
   ```bash
   gh issue create \
     --repo treytomes/ecma_basic \
     --title "Fix nullable errors in parsers" \
     --body-file issue-body.md \
     --label bug,modernization \
     --milestone v0.3.0
   ```

5. **Link Related Issues**
   - Use "Relates to #N" in issue body
   - Create parent/child relationships for broken-down issues
   - Reference similar closed issues for context

## Maintenance

### When Templates Need Updates

After learning new patterns:
```bash
# Update template with discovered patterns
Edit .claude/skills/create-modernization-issue/templates/nullable.md

# Add new pattern to "Common Patterns" section
```

### When Scope Thresholds Change

Edit `SKILL.md` to adjust:
- Small: < N hours
- Medium: N-M hours
- Large: > M hours

Based on team velocity and preferences.

## Related

- `/audit-build` - Generates data used by this skill
- `/commit-and-push` - Use after completing issues
- `.claude/rules/github-workflow.md` - Issue-driven development process
