---
name: verify-coverage
description: Run tests and verify code coverage meets 80% minimum requirement
user-invocable: true
argument-hint: ""
---

# Verify Coverage Skill

Runs the test suite with code coverage analysis and verifies that coverage meets the 80% minimum requirement.

## Usage

```bash
/verify-coverage
```

## What It Does

1. Runs all tests with XPlat Code Coverage
2. Generates coverage report
3. Parses coverage percentage
4. **Fails** if coverage < 80%
5. **Succeeds** if coverage ≥ 80%

## Implementation

I'll run the test suite with coverage collection and verify the results:

```bash
cd src
dotnet test ECMABasic.sln \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --verbosity minimal
```

Then I'll:
- Parse the coverage.cobertura.xml file
- Extract line coverage percentage
- Report pass/fail based on 80% threshold

## Output

**If coverage ≥ 80%:**
```
✅ Code coverage: 82.5%
✅ Meets minimum requirement (80%)
```

**If coverage < 80%:**
```
❌ Code coverage: 76.3%
❌ Below minimum requirement (80%)
⚠️  Need 3.7% more coverage
```

## Related Commands

- `./test.sh` - Full test run with HTML report generation
- `dotnet test --collect:"XPlat Code Coverage"` - Direct test command
