# ECMABasic Modernization Plan

## Overview

This document outlines the modernization tasks required to update ECMABasic from .NET 6.0 to .NET 10 with modern C# patterns and practices.

## Completed Tasks

- ✅ Upgraded all projects to .NET 10
- ✅ Enabled nullable reference types (`<Nullable>enable</Nullable>`)
- ✅ Enabled treat warnings as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`)
- ✅ Enabled code style enforcement (`<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>`)
- ✅ Updated NuGet packages to latest versions
- ✅ Added comprehensive `.editorconfig` for code style
- ✅ Created `.claude` folder with development guidelines
- ✅ Set up GitHub Actions CI/CD pipelines
- ✅ Established spec-driven development process

## Build Status

Initial build after upgrade: **❌ FAILED**

**Total Errors**: ~170 compiler errors + style warnings

### Error Breakdown

1. **Nullable Reference Type Errors** (~85 errors)
   - CS8625: Cannot convert null literal to non-nullable type
   - CS8603: Possible null reference return
   - CS8602: Dereference of a possibly null reference
   - CS8604: Possible null reference argument
   - CS8618: Non-nullable field must contain non-null value
   - CS8600: Converting null literal or possible null value
   - CS8601: Possible null reference assignment

2. **Code Style Errors** (~30 errors)
   - IDE0040: Accessibility modifiers required
   - File-scoped namespace conversions needed

3. **Modern C# Pattern Opportunities**
   - Target-typed `new()` expressions
   - Pattern matching improvements
   - Collection expressions (C# 12)
   - Primary constructors consideration

## Next Steps (Spec-Driven Approach)

Per our development philosophy, we **cannot modify source code without specifications**. We need to create GitHub issues for each category of modernization work:

### Issue 1: Fix Nullable Reference Type Violations
**Type**: Bug  
**Files Affected**: ~40 files in ECMABasic.Core  
**Description**: Address all CS86xx nullable reference type warnings  
**Acceptance Criteria**:
- All nullable reference type errors resolved
- Proper use of `?` for nullable types
- Proper null checks before dereferencing
- String parameters marked nullable where appropriate
- All tests pass

### Issue 2: Add Required Accessibility Modifiers
**Type**: Enhancement  
**Files Affected**: Exception classes, expression classes, IEnvironment.cs  
**Description**: Add explicit `public` modifiers per EditorConfig rules (IDE0040)  
**Acceptance Criteria**:
- All types have explicit accessibility modifiers
- All interface members have explicit modifiers
- EditorConfig rules satisfied
- All tests pass

### Issue 3: Convert to File-Scoped Namespaces
**Type**: Enhancement  
**Files Affected**: All 128 C# files  
**Description**: Convert from block-scoped to file-scoped namespace declarations (C# 10 feature)  
**Before**: `namespace ECMABasic.Core { ... }`  
**After**: `namespace ECMABasic.Core;`  
**Acceptance Criteria**:
- All files use file-scoped namespaces
- All tests pass
- EditorConfig validation passes

### Issue 4: Apply Modern C# Patterns
**Type**: Enhancement  
**Description**: Update code to use modern C# patterns where beneficial  
**Sub-tasks**:
- Target-typed `new()` expressions
- Pattern matching over type checks
- Simplified using statements
- Collection expressions (where beneficial)
**Acceptance Criteria**:
- Modern patterns applied consistently
- Code is more readable
- All tests pass
- Test coverage remains ≥ 80%

### Issue 5: NuGet Package Management Automation
**Type**: Enhancement  
**Description**: Add Dependabot or similar for automated NuGet updates  
**Acceptance Criteria**:
- Automated PR creation for package updates
- Security vulnerability scanning
- CI validates package updates

## Development Process

For each issue above:

1. **Create GitHub Issue** with acceptance criteria
2. **Write Failing Tests** (if behavior changes)
3. **Implement Changes** to satisfy tests and acceptance criteria
4. **Verify**:
   - `dotnet build` succeeds (no warnings)
   - `dotnet test` passes (all tests green)
   - Code coverage ≥ 80%
5. **Create Pull Request** referencing the issue
6. **CI Pipeline** validates build, tests, coverage
7. **Merge** when approved and CI passes

## Timeline Estimate

- Issue 1 (Nullable): 4-6 hours (most critical, blocks other work)
- Issue 2 (Accessibility): 1-2 hours (find/replace mostly)
- Issue 3 (Namespaces): 2-3 hours (automated refactoring available)
- Issue 4 (Patterns): 3-5 hours (manual review and updates)
- Issue 5 (Automation): 1 hour (configuration)

**Total**: ~15-20 hours of development work

## Notes

- All changes must maintain backward compatibility with existing .BAS programs
- ECMA-55 compliance must be preserved
- Test coverage cannot decrease below 80%
- All changes go through PR review process
- CI pipeline blocks merges on failures
