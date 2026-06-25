# Run REPL Test Skill

Quickly run REPL integration tests with pattern matching and verbose output.

## Usage

```bash
/run-repl-test [pattern]
```

**Examples**:
```bash
/run-repl-test FND              # Run tests matching "FND"
/run-repl-test DefFn            # Run all DEF FN tests
/run-repl-test                  # Run all REPL integration tests
```

## What It Does

Executes REPL integration tests with:
- ✅ Automatic test name pattern matching
- ✅ Detailed verbose output
- ✅ Filtered to ReplIntegrationTests class
- ✅ Quick feedback loop for REPL debugging

## Command Template

```bash
dotnet test test/ECMABasic.Test/ECMABasic.Test.csproj \
  --filter "FullyQualifiedName~ReplIntegrationTests[.pattern]" \
  --logger "console;verbosity=detailed"
```

## Examples

### Run Specific Test
```
User: /run-repl-test FND

Me: Running REPL test matching "FND"...

    dotnet test test/ECMABasic.Test/ECMABasic.Test.csproj \
      --filter "FullyQualifiedName~ReplIntegrationTests.Repl_DefFn_SingleToken_FND" \
      --logger "console;verbosity=detailed"

    ✅ Passed: Repl_DefFn_SingleToken_FND [145 ms]
    
    Test Run Successful.
    Total tests: 1
         Passed: 1
     Total time: 0.9 seconds
```

### Run Category
```
User: /run-repl-test DefFn

Me: Running REPL tests matching "DefFn"...

    ✅ Passed: Repl_DefFn_SingleToken_FND
    ✅ Passed: Repl_DefFn_TwoTokens_FN_D
    ✅ Passed: Repl_DefFn_SingleToken_FNA
    ✅ Passed: Repl_DefFn_TwoTokens_FN_A
    ✅ Passed: Repl_DefFn_SingleToken_FNZ
    
    Test Run Successful.
    Total tests: 5
         Passed: 5
     Total time: 1.2 seconds
```

### Run All REPL Tests
```
User: /run-repl-test

Me: Running all REPL integration tests...

    ✅ Passed: 12 tests
    
    Test Run Successful.
    Total tests: 12
         Passed: 12
     Total time: 2.1 seconds
```

## Pattern Matching

The filter uses `FullyQualifiedName~` for substring matching:

| Pattern | Matches |
|---------|---------|
| `FND` | `Repl_DefFn_SingleToken_FND` |
| `DefFn` | All `Repl_DefFn_*` tests |
| `TwoTokens` | All `*_TwoTokens_*` tests |
| `ForLoop` | `Repl_ForLoop_*` tests |
| (none) | All `ReplIntegrationTests.*` |

## Why This Skill Exists

**Problem**: Typing long test filter commands repeatedly during REPL debugging

**Before** (manual):
```bash
dotnet test test/ECMABasic.Test/ECMABasic.Test.csproj \
  --filter "FullyQualifiedName~ReplIntegrationTests.Repl_DefFn_SingleToken_FND" \
  --logger "console;verbosity=detailed"
```

**After** (skill):
```bash
/run-repl-test FND
```

**Time Saved**: ~30 seconds per test run × 20-30 runs per debugging session = 10-15 minutes per bug

## Related Skills

- `/audit-build` - Check for compilation errors
- `./test.sh` - Run full test suite with coverage

## Implementation Notes

When invoked, I will:

1. **Determine filter pattern**:
   - No args: Run all `ReplIntegrationTests.*`
   - With pattern: Filter to `ReplIntegrationTests.{pattern}`

2. **Build test command**:
   ```bash
   dotnet test test/ECMABasic.Test/ECMABasic.Test.csproj \
     --filter "FullyQualifiedName~ReplIntegrationTests${pattern}" \
     --logger "console;verbosity=detailed"
   ```

3. **Execute and parse output**:
   - Show test names and pass/fail status
   - Display execution time
   - Report summary

4. **Handle errors**:
   - No matching tests: Show available REPL tests
   - Build failures: Suggest running `./build.sh`
   - Test failures: Show detailed failure messages

## Quick Reference

```bash
# Single test
/run-repl-test FND

# Category
/run-repl-test DefFn

# All tests
/run-repl-test

# With specific test method name
/run-repl-test Repl_DefFn_SingleToken_FND
```

## Tips

- **During debugging**: Run specific test repeatedly with `/run-repl-test TestName`
- **After changes**: Run category with `/run-repl-test Category` to verify related tests
- **Before commit**: Run all with `/run-repl-test` to ensure nothing broke
- **Combine with watch**: Use `dotnet watch test --filter ...` for continuous testing
