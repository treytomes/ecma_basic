---
name: spec-first
description: Enforce specification-first development - no source changes without tests/specs
paths: ["src/**/*.cs"]
---

# Specification-First Development

## Core Principle

**Never modify production source code without a specification to work against.**

## Before Changing Source Code

You MUST have one of the following:

### 1. Existing Test (Red-Green-Refactor)
- A failing test exists that defines the expected behavior
- The test was written BEFORE the implementation
- The test serves as the executable specification

### 2. GitHub Issue with Acceptance Criteria
- A GitHub issue exists describing the feature/bug
- Issue includes clear, testable acceptance criteria
- Issue has been reviewed and approved (if required)

### 3. ECMA Standard Reference
- Change aligns with ECMA-55 or ECMA-116 specification
- Reference the relevant section of the standard
- Ensure corresponding test exists in the test suite

## Workflow for New Features

```
1. CREATE GitHub issue with acceptance criteria
2. WRITE failing test(s) that validate acceptance criteria
3. VERIFY test fails (Red)
4. IMPLEMENT minimal code to pass test (Green)
5. REFACTOR to improve code quality
6. ENSURE all tests still pass
7. COMMIT with reference to issue number
```

## Workflow for Bug Fixes

```
1. CREATE GitHub issue describing the bug
2. WRITE test that reproduces the bug
3. VERIFY test fails (demonstrates bug)
4. FIX the bug
5. VERIFY test now passes
6. ADD regression test to prevent future occurrences
7. COMMIT with "Fixes #N" in message
```

## Exceptions (Rare)

The only acceptable source changes WITHOUT a spec:

### Refactoring ONLY If:
- No behavior change whatsoever
- All existing tests pass unchanged
- Code coverage remains the same or improves
- Changes improve code quality (readability, maintainability, performance)

### Examples of Acceptable Refactoring:
- Renaming variables for clarity (no behavior change)
- Extracting methods to reduce complexity
- Removing dead code
- Updating dependencies/packages
- Reformatting code style

### Examples of UNACCEPTABLE Changes Without Spec:
- Adding new features
- Modifying logic or algorithms
- Changing method signatures
- Adding/removing parameters
- Fixing bugs (even obvious ones - write test first!)
- Performance optimizations (prove it with a test)

## When Claude Proposes Source Changes

If Claude suggests changing source code, Claude MUST:

1. **Check for existing test**: Search for tests covering this code
2. **Check for GitHub issue**: Ask if an issue exists
3. **If neither exists**: 
   - STOP and ask user to create issue with acceptance criteria
   - OR offer to create a test first before implementation
   - NEVER proceed directly to implementation

## Response Template

When asked to modify source without a spec:

```
I need a specification before modifying the source code. We follow spec-driven development.

Let's approach this by:

Option 1: Create GitHub Issue
- Document the feature/bug with acceptance criteria
- Then I'll write tests based on those criteria
- Finally, implement to pass the tests

Option 2: Write Test First
- I'll write a failing test that defines the expected behavior
- Then implement to make it pass

Which approach would you prefer?
```

## Validating Compliance

Before ANY source file edit in `src/**/*.cs`:

```
❌ STOP: Do I have a specification?
  ✓ Existing failing test?
  ✓ GitHub issue with acceptance criteria?
  ✓ ECMA standard reference?
  ✓ Pure refactoring with no behavior change?

❌ If NO to all → ASK USER for specification first

✅ If YES → Proceed with implementation
```

## Benefits

- **Quality**: Tests define correct behavior before code exists
- **Confidence**: Changes are validated against specifications
- **Documentation**: Tests serve as living documentation
- **Regression Prevention**: Issues have tests preventing recurrence
- **Design**: Thinking through tests first improves API design
