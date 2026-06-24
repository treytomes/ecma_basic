---
name: commit-and-push
description: Build, test, group changes logically, commit with Conventional Commits format, and push
user-invocable: true
argument-hint: ""
---

# Commit and Push Skill

Automates the complete commit workflow: verification → grouping → commit → push.

## Usage

```bash
/commit-and-push
```

## What It Does

### Phase 1: Pre-Commit Verification ✅

1. **Check git status**
   - Identify all changed/added/deleted files
   - Ensure we're on the correct branch
   - Verify no conflicts exist

2. **Build verification**
   ```bash
   dotnet build src/ECMABasic.sln --configuration Release
   ```
   - Must succeed with zero warnings
   - Fails if any compilation errors

3. **Test verification**
   ```bash
   dotnet test src/ECMABasic.sln --configuration Release
   ```
   - All tests must pass
   - No skipped or failing tests

4. **Coverage check**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
   - Must meet 80% minimum
   - Fails if coverage drops below threshold

### Phase 2: Change Grouping 📦

Analyzes changed files and groups them **logically** by:

#### Group by Type
- **Source code**: `src/**/*.cs`
- **Tests**: `src/**/*Test*.cs`, `**/Tests/**`
- **Configuration**: `*.csproj`, `*.json`, `*.yml`
- **Documentation**: `*.md`, `docs/**`
- **Scripts**: `*.sh`, `*.bat`
- **Claude config**: `.claude/**`

#### Group by Feature Area
- **Core**: `ECMABasic.Application/**`
- **REPL**: `ECMABasic55/**`
- **Tests**: `ECMABasic.Test/**`
- **Infrastructure**: Build, CI/CD files

#### Avoid Mixed Commits
❌ Don't mix:
- Source + documentation
- Multiple unrelated features
- Refactoring + bug fix
- Test + implementation (unless paired)

✅ Group together:
- Related source files for one feature
- Test + implementation for same feature
- Documentation for same feature
- Configuration changes for one purpose

### Phase 3: Commit Generation 📝

For each logical group, I'll:

1. **Determine commit type**
   - Analyze changes to choose: feat, fix, refactor, test, docs, etc.
   - Examine file contents, not just names

2. **Generate commit message**
   - Follow Conventional Commits format
   - Use imperative mood
   - Keep subject ≤ 72 characters
   - Include body with WHY explanation
   - Reference issue numbers if applicable

3. **Present for approval**
   ```
   Commit 1 of 3:
   
   feat(parser): add MID$ function support
   
   Implements the MID$ string function per ECMA-55 specification.
   Supports both 2-argument (MID$(str, start)) and 3-argument
   (MID$(str, start, length)) forms.
   
   Closes #47
   
   Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
   
   Files:
   - src/ECMABasic.Application/FunctionFactory.cs
   - src/ECMABasic.Test/FunctionTests.cs
   
   [A]pprove [E]dit [S]kip [Q]uit?
   ```

4. **Execute approved commits**
   ```bash
   git add <files>
   git commit -m "message"
   ```

### Phase 4: Push 🚀

After all commits are approved:

1. **Verify remote**
   ```bash
   git remote -v
   ```

2. **Push to origin**
   ```bash
   git push origin <branch-name>
   ```

3. **Confirm success**
   - Show push result
   - Display pushed commit SHAs

## Grouping Examples

### Example 1: Nullable Fix
**Changed files**:
- `src/ECMABasic.Application/CharacterReader.cs`
- `src/ECMABasic.Application/Interpreter.cs`
- `src/ECMABasic.Test/InterpreterTests.cs`

**Grouping**:
```
Group 1: Core nullable fixes
- CharacterReader.cs
- Interpreter.cs

Commit: fix(core): resolve nullable reference warnings
Issue: #1

Group 2: Test updates
- InterpreterTests.cs

Commit: test: update tests for nullable changes
Related: #1
```

### Example 2: Mixed Changes
**Changed files**:
- `src/ECMABasic.Application/Parser.cs` (new feature)
- `src/ECMABasic55/Program.cs` (bug fix)
- `README.md` (documentation)
- `.claude/rules/modernization.md` (config)

**Grouping**:
```
Group 1: New feature
- Parser.cs
Commit: feat(parser): add WHILE-WEND loop support

Group 2: Bug fix  
- Program.cs
Commit: fix(repl): handle empty input gracefully

Group 3: Documentation
- README.md
Commit: docs: update README with WHILE-WEND info

Group 4: Configuration
- .claude/rules/modernization.md
Commit: chore: add WHILE-WEND pattern to rules
```

## Interactive Workflow

```
User: /commit-and-push

Me: 🔍 Analyzing changes...
    Found 12 changed files

    ✅ Build successful (0 warnings)
    ✅ All tests passed (87/87)
    ✅ Coverage: 82.3% (meets 80% requirement)
    
    📦 Grouped into 3 logical commits:

    ─────────────────────────────────────────
    Commit 1 of 3: Core Changes
    ─────────────────────────────────────────
    
    fix(core): resolve nullable reference warnings
    
    Fixed CS8625 and CS8603 errors in CharacterReader and
    Interpreter classes by adding proper nullable annotations
    and null checks.
    
    Closes #1
    
    Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
    
    Files (8):
    - src/ECMABasic.Application/CharacterReader.cs
    - src/ECMABasic.Application/Interpreter.cs
    - [... 6 more]
    
    [A]pprove [E]dit message [S]kip [Q]uit? _

User: A

Me: ✅ Committed: fix(core): resolve nullable reference warnings
    SHA: abc1234
    
    ─────────────────────────────────────────
    Commit 2 of 3: Test Updates
    ─────────────────────────────────────────
    
    test: add coverage for nullable edge cases
    
    Added tests to verify proper null handling in CharacterReader
    and Interpreter after nullable reference type fixes.
    
    Related to #1
    
    Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
    
    Files (3):
    - src/ECMABasic.Test/CharacterReaderTests.cs
    - src/ECMABasic.Test/InterpreterTests.cs
    - src/ECMABasic.Test/Resources/null-test.BAS
    
    [A]pprove [E]dit message [S]kip [Q]uit? _

User: A

Me: ✅ Committed: test: add coverage for nullable edge cases
    SHA: def5678
    
    ─────────────────────────────────────────
    Commit 3 of 3: Documentation
    ─────────────────────────────────────────
    
    docs: update MODERNIZATION_PLAN with progress
    
    Marked Issue #1 (nullable fixes) as completed.
    Updated coverage metrics to reflect current 82.3%.
    
    Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
    
    Files (1):
    - docs/MODERNIZATION_PLAN.md
    
    [A]pprove [E]dit message [S]kip [Q]uit? _

User: A

Me: ✅ Committed: docs: update MODERNIZATION_PLAN with progress
    SHA: ghi9012
    
    📤 Ready to push 3 commits
    
    Push to origin/feature/1-fix-nullable-warnings? [Y/n] _

User: Y

Me: 🚀 Pushing...
    
    ✅ Pushed successfully!
    
    Summary:
    - 3 commits pushed
    - 12 files changed
    - Branch: feature/1-fix-nullable-warnings
    - Remote: origin
```

## Failure Handling

### Build Fails
```
❌ Build failed with 5 errors
Cannot commit until build succeeds.

Fix errors and run /commit-and-push again.
```

### Tests Fail
```
❌ 3 tests failed
Cannot commit until all tests pass.

Failed tests:
- InterpreterTests.Parse_NullInput_ThrowsException
- CharacterReaderTests.Read_EmptyStream_ReturnsNull
- ForStatementTests.Execute_NegativeStep_Works

Fix tests and run /commit-and-push again.
```

### Coverage Below Threshold
```
⚠️ Coverage: 78.5% (below 80% minimum)
Cannot commit - coverage requirement not met.

Need 1.5% more coverage (approximately 15-20 more lines tested).
Run /verify-coverage for detailed report.
```

### No Changes
```
ℹ️ No changes to commit
Working directory is clean.
```

### Conflicts
```
❌ Merge conflicts detected in:
- src/ECMABasic.Application/Interpreter.cs

Resolve conflicts manually:
1. git status
2. Edit conflicted files
3. git add <resolved-files>
4. Run /commit-and-push again
```

## Options & Flags

### Auto-approve Mode (Use with Caution)
```bash
# Skip approval prompts, commit all groups automatically
/commit-and-push --auto
```

**Warning**: Only use when:
- Changes are simple and obvious
- You trust the grouping
- You've reviewed changes beforehand

### Dry Run
```bash
# Show what would be committed without actually committing
/commit-and-push --dry-run
```

### Single Commit Mode
```bash
# Force all changes into a single commit (not recommended)
/commit-and-push --single
```

## Safety Features

✅ **Build verification** - Won't commit if build fails
✅ **Test verification** - Won't commit if tests fail  
✅ **Coverage check** - Won't commit if coverage < 80%
✅ **Approval prompts** - Review each commit before creation
✅ **Push confirmation** - Explicit approval before pushing
✅ **Rollback support** - Can undo commits before push

## When to Use

✅ **Use /commit-and-push when**:
- You've completed a feature or fix
- All changes are related and tested
- Build and tests are passing
- You're ready to push to remote

❌ **Don't use when**:
- Work in progress (not ready to share)
- Experimental code (use manual commits)
- Need to squash/amend (use git directly)
- Complex merge scenarios

## Related Commands

- `/verify-coverage` - Check coverage before committing
- `/audit-build` - See if any warnings exist
- Manual git: `git status`, `git add`, `git commit`, `git push`

## Configuration

Configured via `.claude/rules/git-conventions.md`:
- Commit message format
- Co-authored-by template
- Subject line length limits
- Branch naming conventions

## Important Notes

- **I never force-push** - Uses `git push`, never `git push --force`
- **Hooks run** - Pre-commit hooks execute normally
- **GPG signing** - Respects your git config (doesn't skip)
- **Approval required** - Won't commit without your permission
- **Reversible** - Before push, you can `git reset` to undo

## Best Practices

1. **Review changes first**
   ```bash
   git status
   git diff
   ```

2. **Run /commit-and-push** when ready

3. **Review each commit** before approving

4. **Verify push** was successful

5. **Create PR** if on feature branch
