---
name: git-conventions
description: Git commit message formatting and repository conventions
paths: ["**/*"]
---

# Git Conventions

## Conventional Commits Format

All commit messages MUST follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Commit Types

| Type | When to Use | Example |
|------|-------------|---------|
| `feat` | New feature | `feat(parser): add support for MID$ function` |
| `fix` | Bug fix | `fix(interpreter): handle null in FOR statement` |
| `docs` | Documentation only | `docs: update README with .NET 10 info` |
| `style` | Code style (formatting, no logic change) | `style: convert to file-scoped namespaces` |
| `refactor` | Code change that neither fixes nor adds | `refactor(core): extract parser logic to separate class` |
| `perf` | Performance improvement | `perf(tokenizer): optimize string allocation` |
| `test` | Adding or updating tests | `test: add coverage for nullable edge cases` |
| `build` | Build system or dependencies | `build: upgrade to .NET 10` |
| `ci` | CI/CD configuration | `ci: add code coverage enforcement` |
| `chore` | Other changes (tooling, configs) | `chore: update .editorconfig rules` |
| `revert` | Revert a previous commit | `revert: revert "feat: add experimental feature"` |

### Scope (Optional)

Scope indicates which part of the codebase is affected:

- `parser` - Parsing logic
- `interpreter` - Interpreter execution
- `core` - Core library
- `repl` - REPL/console interface
- `test` - Test infrastructure
- `ci` - CI/CD pipeline
- `docs` - Documentation

**Examples**:
```
feat(parser): add WHILE-WEND loop support
fix(interpreter): prevent stack overflow in recursive GOSUB
test(core): increase coverage for expression parser
```

### Description

- Use imperative mood: "add" not "added" or "adds"
- Don't capitalize first letter: "add feature" not "Add feature"
- No period at the end
- Keep under 72 characters

✅ Good:
```
feat: add nullable reference type support
fix: resolve CS8625 in CharacterReader
docs: update architecture decision in wiki
```

❌ Bad:
```
Added nullable reference type support.    // Past tense, ends with period
Fix: Resolve CS8625 In CharacterReader   // Capitalized
Updated docs                              // Vague, no type
```

### Body (Optional but Recommended)

Explain the **why** behind the change, not the what (code shows what):

```
fix(interpreter): handle null in FOR statement

The FOR statement was dereferencing the step expression without checking
for null, causing NullReferenceException when STEP was omitted.

Added null check and default step value of 1 per ECMA-55 specification.

Fixes #42
```

**Include**:
- Why this change is necessary
- What problem it solves
- Any side effects or caveats
- References to issues or specs

**Don't include**:
- What the code does (code is self-documenting)
- Implementation details (review the diff)

### Footer

**Required for Claude commits**:
```
Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

**Link to issues**:
```
Fixes #123
Closes #456
Relates to #789
```

**Breaking changes**:
```
BREAKING CHANGE: remove deprecated BASIC-1 extensions

The BASIC-1 non-standard extensions (WHILE, UNTIL) have been removed.
Use ECMA-55 standard FOR-NEXT loops instead.

Migration: Replace WHILE loops with FOR loops or refactor to GOTO.
```

## Complete Examples

### Simple Feature
```
feat: add var keyword preference to .editorconfig

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

### Bug Fix with Context
```
fix(parser): prevent null dereference in ReadStatement

ReadStatement was not checking if the input variable list was empty
before iterating, causing NullReferenceException in edge cases.

Added validation that throws SyntaxException if no variables provided,
matching ECMA-55 specification requirements.

Fixes #156

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

### Refactoring
```
refactor(core): convert all files to file-scoped namespaces

Modernizes codebase to use C# 10 file-scoped namespace syntax,
reducing indentation and improving readability.

No functional changes - pure syntax update.

Closes #3

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

### Documentation
```
docs: add modernization plan and setup guides

Created comprehensive documentation for .NET 10 migration:
- MODERNIZATION_PLAN.md outlines 5 phases
- SETUP_COMPLETE.md describes current configuration
- CLAUDE_SETUP_GUIDE.md explains .claude folder structure

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

### Breaking Change
```
feat!: upgrade to .NET 10 with nullable reference types

BREAKING CHANGE: All projects now target .NET 10 and enable nullable
reference types. This requires:
- .NET 10 SDK installed
- Code changes to annotate nullable types
- Existing null checks may need updating

Migration: Install .NET 10 SDK and fix nullable warnings.

Closes #1

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

## Commit Frequency

### Do Commit
✅ After completing a discrete piece of work
✅ After fixing a specific bug
✅ After adding a complete feature (even if small)
✅ When tests pass and code builds
✅ Before switching context to different work

### Don't Commit
❌ Work in progress that doesn't compile
❌ Half-implemented features
❌ Code with failing tests (unless WIP branch)
❌ Debug code, commented-out experiments
❌ Multiple unrelated changes in one commit

## Branch Naming

Follow the pattern:
```
<type>/<issue-number>-<short-description>

Examples:
feature/1-fix-nullable-warnings
fix/42-null-reference-in-for-statement
refactor/3-file-scoped-namespaces
docs/update-readme
```

## Commit Message Length

- **Subject line**: ≤ 72 characters (enforced)
- **Body lines**: ≤ 100 characters (wrapped)
- **No limit on footer**

## Multi-Author Commits

When pair programming or AI-assisted:
```
feat: add comprehensive test coverage

Implemented unit tests for CharacterReader class achieving 92% coverage.

Co-Authored-By: Trey Tomes <email@example.com>
Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>
```

## Verification

Before committing, verify:
- [ ] Message follows Conventional Commits format
- [ ] Type is correct (feat, fix, etc.)
- [ ] Description is imperative, lowercase, no period
- [ ] Body explains WHY (if needed)
- [ ] Issue references included (if applicable)
- [ ] Co-Authored-By included for Claude commits
- [ ] Subject line ≤ 72 characters

## Tools

### Git Hook (Optional)

Create `.git/hooks/commit-msg`:
```bash
#!/bin/sh
commit_msg_file=$1
commit_msg=$(cat "$commit_msg_file")

# Check if message follows Conventional Commits
if ! echo "$commit_msg" | grep -qE "^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?: .+"; then
  echo "❌ Commit message must follow Conventional Commits format"
  echo "Format: <type>[optional scope]: <description>"
  exit 1
fi

# Check subject line length
subject=$(echo "$commit_msg" | head -n1)
if [ ${#subject} -gt 72 ]; then
  echo "❌ Subject line must be ≤ 72 characters (currently: ${#subject})"
  exit 1
fi

echo "✅ Commit message format valid"
```

### Validation Command

```bash
# Validate last commit message
git log -1 --pretty=%B | head -n1 | grep -E "^(feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?: .+"
```

## References

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Angular Commit Guidelines](https://github.com/angular/angular/blob/master/CONTRIBUTING.md#commit)
- [Semantic Versioning](https://semver.org/)

## Examples from This Project

```
feat(core): add nullable reference type annotations
fix(parser): handle empty input in ReadStatement
docs: update CLAUDE.md with spec-driven development
style: apply var keyword across all files
refactor(interpreter): simplify expression evaluation
test: increase coverage for nullable edge cases
build: upgrade all projects to .NET 10
ci: add code coverage enforcement to GitHub Actions
chore: update .editorconfig with var preferences
```

## When in Doubt

Use `feat` or `fix`:
- Adding something → `feat`
- Fixing something → `fix`
- Everything else → check the table above
