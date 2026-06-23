---
name: audit-build
description: Analyze build errors and categorize them by type for planning fixes
user-invocable: true
argument-hint: ""
---

# Audit Build Skill

Runs a build, captures all errors and warnings, then categorizes them to help plan modernization work.

## Usage

```bash
/audit-build
```

## What It Does

1. Attempts to build the solution
2. Captures all compiler errors and warnings
3. Categorizes them:
   - **Nullable Reference Types** (CS86xx)
   - **Code Style** (IDExxxx)
   - **Other Compiler Errors** (CSxxxx)
4. Counts errors by category
5. Groups by file
6. Provides summary and recommendations

## Output Format

```
🔍 Build Audit Report
═══════════════════════════════════════

📊 Error Summary
─────────────────────────────────────
Total Errors: 170
  • Nullable Reference: 85 errors (50%)
  • Code Style: 30 errors (18%)
  • Other: 55 errors (32%)

📁 Most Affected Files
─────────────────────────────────────
  1. CharacterReader.cs         - 15 errors
  2. Interpreter.cs             - 12 errors
  3. ComplexTokenReader.cs      - 10 errors
  4. NumericExpressionParser.cs - 9 errors
  5. EnvironmentBase.cs         - 8 errors

🏷️  By Category
─────────────────────────────────────

Nullable Reference Types (85):
  • CS8625: Null literal to non-nullable   - 25
  • CS8603: Possible null return           - 20
  • CS8602: Possible null dereference      - 18
  • CS8604: Possible null argument         - 12
  • CS8618: Non-nullable field             - 10

Code Style (30):
  • IDE0040: Accessibility modifiers       - 30

💡 Recommendations
─────────────────────────────────────
Priority Order:
  1. Fix nullable violations (blocks other work)
  2. Add accessibility modifiers (quick wins)
  3. Convert to file-scoped namespaces (automated)
  4. Apply modern patterns (ongoing)

📋 Next Steps
─────────────────────────────────────
Create GitHub issues for:
  • Issue #1: Fix nullable reference types in top 10 files
  • Issue #2: Add accessibility modifiers (all files)
  • Issue #3: File-scoped namespace conversion

Estimated effort: 15-20 hours total
```

## Use Cases

- **Initial Assessment**: Understand scope before starting
- **Progress Tracking**: See how many errors remain
- **Sprint Planning**: Prioritize which files to fix first
- **Impact Analysis**: See what changed after merging fixes

## Related Commands

- `./build.sh` - Standard build without analysis
- `/verify-coverage` - Check test coverage
