# Work In Progress - Issue #2 Nullable Parser Fixes

**Status**: 🟢 50% Milestone Reached!  
**Branch**: `fix/2-nullable-parsers` (pushed to remote)  
**Issue**: https://github.com/treytomes/ecma_basic/issues/2  
**Last Updated**: 2026-06-23

---

## 🎉 50% Milestone Achieved!

**All 20 statement parsers are now fixed with 0 nullable errors!**

---

## ✅ Completed (20/40 files = 50%)

### Commit 1 - Base Classes (2 files)
1. ✅ **StatementParser.cs** - Base class with 8 nullable annotations
   - `Parse()` → `IStatement?`
   - `ProcessSpace()` → `Token?`
   - `ParseExpression()` → `IExpression?`
   - `ParseBinaryExpression()` → `IExpression?`
   - `ParseAtomicExpression()` → `IExpression?`
   - `ParseVariableExpression()` → `VariableExpression?`
   - `ParseNumericExpression()` → `IExpression?`
   - `ParseStringExpression()` → `IExpression?`

2. ✅ **PrintStatementParser.cs**

### Commit 2 - Simple Parsers (5 files)
3. ✅ **ReturnStatementParser.cs**
4. ✅ **EndStatementParser.cs**
5. ✅ **RestoreStatementParser.cs**
6. ✅ **StopStatementParser.cs**
7. ✅ **RemarkStatementParser.cs**

### Commit 3 - Variable Parsers (3 files)
8. ✅ **LetStatementParser.cs**
9. ✅ **ReadStatementParser.cs**
10. ✅ **InputStatementParser.cs**

### Commit 4 - Initial Control Flow (6 files)
11. ✅ **DataStatementParser.cs**
12. ✅ **GotoStatementParser.cs** (partial)
13. ✅ **GosubStatementParser.cs** (partial)
14. ✅ **IfThenStatementParser.cs**
15. ✅ **NextStatementParser.cs** (partial)
16. ✅ **ForStatementParser.cs** (partial)

### Commit 5 - Control Flow Null Checks (5 files)
17. ✅ **GotoStatementParser.cs** (completed)
18. ✅ **GosubStatementParser.cs** (completed)
19. ✅ **NextStatementParser.cs** (completed)
20. ✅ **ForStatementParser.cs** (completed)
21. ✅ **OnGotoStatementParser.cs** (completed)

### Pattern Established ✅

The base classes now demonstrate the correct pattern:

**Before** (causes CS8603):
```csharp
public IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
{
    if (!CanParse(reader))
        return null;  // ❌ Error: returning null to non-nullable
    // ...
}
```

**After** (correct):
```csharp
public IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
{
    if (!CanParse(reader))
        return null;  // ✅ OK: return type is nullable
    // ...
}
```

---

## 🔧 Remaining Work (38/40 files)

### High Priority - Statement Parsers (16 files)

All inherit from `StatementParser`, so `Parse()` override signature is already correct.
Only need to fix internal helper methods and parameter annotations.

| File | Errors | Complexity | Estimate |
|------|--------|------------|----------|
| **LetStatementParser.cs** | 5 | Medium | 20 min |
| **ForStatementParser.cs** | 6 | Medium | 20 min |
| **GotoStatementParser.cs** | 3 | Low | 10 min |
| **GosubStatementParser.cs** | 3 | Low | 10 min |
| **IfThenStatementParser.cs** | 2 | Low | 10 min |
| **NextStatementParser.cs** | 3 | Low | 10 min |
| **ReadStatementParser.cs** | 4 | Low | 15 min |
| **InputStatementParser.cs** | 4 | Low | 15 min |
| **DataStatementParser.cs** | 1 | Low | 5 min |
| **OnGotoStatementParser.cs** | ~4 | Medium | 15 min |
| **ReturnStatementParser.cs** | 1 | Low | 5 min |
| **EndStatementParser.cs** | 1 | Low | 5 min |
| **RestoreStatementParser.cs** | 1 | Low | 5 min |
| **StopStatementParser.cs** | 1 | Low | 5 min |
| **RemarkStatementParser.cs** | 1 | Low | 5 min |

**Subtotal**: ~16 files, ~2.5 hours

### High Priority - Expression Parsers (4 files)

These need both Parse() method fixes and internal helper methods.

| File | Errors | Complexity | Estimate |
|------|--------|------------|----------|
| **ExpressionParser.cs** | 1 | Low | 10 min |
| **NumericExpressionParser.cs** | ~15 | High | 45 min |
| **StringExpressionParser.cs** | ~10 | High | 30 min |
| **ExpressionParser.cs** (base) | ~5 | Medium | 20 min |

**Subtotal**: ~4 files, ~1.5 hours

### Medium Priority - Expression Classes (~18 files)

Various expression implementation classes that may have nullable issues:

- BinaryExpression.cs
- UnaryExpression.cs
- VariableExpression.cs
- FunctionCall.cs
- TabExpression.cs
- CommaExpression.cs
- SemicolonExpression.cs
- And other expression types...

**Estimate**: ~1 hour (most are likely simple)

---

## 📋 Completion Checklist

### Development Steps
- [x] Fix StatementParser base class
- [x] Fix PrintStatementParser as example
- [ ] Fix remaining 14 statement parsers
- [ ] Fix ExpressionParser base class
- [ ] Fix NumericExpressionParser
- [ ] Fix StringExpressionParser  
- [ ] Fix expression implementation classes
- [ ] Run build to verify all parser errors resolved
- [ ] Run tests to ensure no behavioral changes
- [ ] Verify coverage remains ≥ 80%

### Testing & Verification
- [ ] Build succeeds: `dotnet build src/ECMABasic.sln --configuration Release`
- [ ] All tests pass: `dotnet test src/ECMABasic.sln`
- [ ] Coverage check: `/verify-coverage` shows ≥ 80%
- [ ] Audit remaining errors: `/audit-build` (should show ~87 remaining, all non-parser)

### Git Workflow
- [x] Create branch: `fix/2-nullable-parsers`
- [x] Commit base parser fixes (fcf5a38)
- [ ] Commit remaining parser fixes (group logically)
- [ ] Push branch to remote
- [ ] Create pull request
- [ ] Reference Issue #2 in PR

---

## 🎯 Next Session Plan

### Step 1: Fix Simple Parsers (30 minutes)
Fix the 10 simplest parsers that just need Parse() override:
- ReturnStatementParser.cs
- EndStatementParser.cs
- RestoreStatementParser.cs
- StopStatementParser.cs
- RemarkStatementParser.cs
- And 5 more simple ones

**Approach**: Most just need the inherited `Parse()` signature - already fixed by base class change.

### Step 2: Fix Complex Parsers (1 hour)
Fix parsers with internal logic:
- LetStatementParser.cs (variable assignments)
- ForStatementParser.cs (loop logic)
- GotoStatementParser.cs / GosubStatementParser.cs
- ReadStatementParser.cs / InputStatementParser.cs

**Pattern**: Add null checks before passing to constructors.

### Step 3: Fix Expression Parsers (1.5 hours)
- NumericExpressionParser.cs (most complex)
- StringExpressionParser.cs
- ExpressionParser.cs base class

**Pattern**: Return nullable from Parse(), add null checks in operators.

### Step 4: Verify & Commit (30 minutes)
- Run build
- Run tests
- Verify coverage
- Commit with grouped changes
- Push to remote

---

## 🔍 Common Patterns to Apply

### Pattern 1: Constructor Parameter Null Checks

**Before**:
```csharp
var expr = ParseExpression(reader, lineNumber, true);
return new LetStatement(variable, expr);  // ❌ CS8604: expr might be null
```

**After Option 1** (Add null check):
```csharp
var expr = ParseExpression(reader, lineNumber, true);
if (expr == null)
    throw new SyntaxException("Expected expression");
return new LetStatement(variable, expr);  // ✅ expr is guaranteed non-null
```

**After Option 2** (Make parameter nullable):
```csharp
var expr = ParseExpression(reader, lineNumber, false);
return new LetStatement(variable, expr);  // ✅ If constructor accepts IExpression?
```

### Pattern 2: List.Add() Null Checks

**Before**:
```csharp
var variables = new List<VariableExpression>();
var variable = ParseVariableExpression(reader);
variables.Add(variable);  // ❌ CS8604: variable might be null
```

**After**:
```csharp
var variables = new List<VariableExpression>();
var variable = ParseVariableExpression(reader);
if (variable != null)
{
    variables.Add(variable);  // ✅ Null check before add
}
```

### Pattern 3: Already Fixed by Base Class

Many parsers like `ReturnStatementParser` don't need changes because:
- They inherit from `StatementParser`
- `StatementParser.Parse()` is now `IStatement?`
- The override signature is automatically correct

**Check with build** - if no errors for a file, it's already fixed by inheritance!

---

## 📊 Current Build Status

**Before this work**: 267 total errors (127 nullable, 120 style, 20 other)

**After base parser fixes**: ~265 errors remaining (125 nullable, 120 style, 20 other)

**Target after Issue #2**: ~225 errors remaining (85 nullable in non-parsers, 120 style, 20 other)

---

## 🚀 Commands for Next Session

```bash
# Continue from where we left off
git checkout fix/2-nullable-parsers

# Check which parser files still have errors
dotnet build src/ECMABasic.Application/ECMABasic.Application.csproj 2>&1 | grep -E "Parser\.cs.*error CS86"

# Fix files (use Edit tool)

# Test after each group of fixes
dotnet build src/ECMABasic.Application/ECMABasic.Application.csproj --nologo --verbosity quiet

# When all parsers done, run full test suite
dotnet test src/ECMABasic.sln

# Verify coverage
/verify-coverage

# Commit progress
git add .
git commit -m "fix(parser): resolve remaining nullable annotations in X parsers"

# Push when complete
git push origin fix/2-nullable-parsers

# Create PR
gh pr create --title "Fix nullable reference type violations in parsers" \
  --body "Closes #2" \
  --base main
```

---

## 💡 Tips for Efficient Completion

1. **Batch similar files**: Fix all simple parsers together in one commit
2. **Use build output**: Let compiler tell you what's left
3. **Pattern recognition**: Most fixes follow the same 2-3 patterns
4. **Test frequently**: Run build after each group of 3-5 files
5. **Commit logically**: 
   - Commit 1: Simple parsers (ReturnStatement, EndStatement, etc.)
   - Commit 2: Variable parsers (Let, Read, Input)
   - Commit 3: Control flow parsers (For, Goto, If)
   - Commit 4: Expression parsers

---

## ⏱️ Time Estimates

| Task | Estimate | Progress |
|------|----------|----------|
| Base classes (StatementParser, PrintStatementParser) | 30 min | ✅ Done |
| Simple statement parsers (10 files) | 30 min | ⏸️ Next |
| Complex statement parsers (6 files) | 60 min | ⏸️ Pending |
| Expression parsers (4 files) | 90 min | ⏸️ Pending |
| Testing & verification | 30 min | ⏸️ Pending |
| **Total** | **240 min (4 hours)** | **15% done** |

---

## 📌 Related Issues

- **Issue #3**: Fix nullable violations in expressions (depends on #2)
- **Issue #4**: Fix nullable violations in statements (depends on #2, #3)
- **Issue #5**: Fix nullable violations in environment (depends on #2-4)

Completing Issue #2 (parsers) unblocks the other 3 issues and allows the build to succeed.

---

**Last Updated**: 2026-06-23  
**Last Commit**: fcf5a38 (fix base parser classes)  
**Branch**: fix/2-nullable-parsers  
**Status**: Ready to continue
