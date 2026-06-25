---
name: testing
description: Testing standards and code coverage requirements
paths: ["**/*Test*.cs", "**/Tests/**/*.cs"]
---

# Testing Standards

## Code Coverage Requirements

**Minimum 80% code coverage** is required for all production code.

### Measuring Coverage

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report (install reportgenerator first)
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# View report
start coverage-report/index.html
```

### Coverage Guidelines
- Focus on meaningful tests, not just hitting coverage metrics
- Prioritize testing:
  - Public APIs and interfaces
  - Business logic and algorithms
  - Error handling and edge cases
  - Parser and interpreter logic
- Lower priority for coverage:
  - Simple getters/setters
  - Constructor assignments
  - Trivial forwarding methods

## Test Structure

### Organization
- Test classes mirror production class structure
- Group tests by ECMA-55 standard sections (Group1-7)
- Keep test resources in `Resources/` directory

### Naming Conventions
- Test classes: `{ClassName}Tests`
- Test methods: `MethodName_Scenario_ExpectedResult`
- Spec tests: `P{TestNumber}_{Description}` (e.g., `P050_Hello`)

### Test Patterns

#### Arrange-Act-Assert
```csharp
[Fact]
public void Interpreter_ValidProgram_ExecutesSuccessfully()
{
    // Arrange
    var env = new TestEnvironment();
    var program = "10 PRINT \"HELLO\"\n20 END";
    
    // Act
    var result = Interpreter.FromText(program, env);
    
    // Assert
    Assert.True(result);
}
```

#### Specification Tests
```csharp
[Fact]
public void P050_Hello()
{
    // Tests are named by ECMA-55 test case number
    var result = RunProgram("P050-HELLO.BAS");
    var expected = File.ReadAllText("Resources/P050-HELLO.OK");
    Assert.Equal(expected, result);
}
```

## Test Categories

### Unit Tests
- Test individual components in isolation
- Mock dependencies via constructor injection
- Fast execution (milliseconds)
- No I/O, no external dependencies

### Integration Tests
- Test component interactions
- Use real implementations where practical
- Test full interpreter pipeline

### Specification Tests
- Validate ECMA-55 standard compliance
- Sample BASIC programs with expected output
- Ensure backward compatibility

### REPL Integration Tests

**CRITICAL**: The REPL code path (`RuntimeInterpreter.ProcessImmediate()`) differs from batch mode (`Interpreter.FromText()`). Features that work in batch mode may fail in the REPL due to different initialization and state management.

#### When to Use REPL Integration Tests
- Testing features used in interactive mode (DEF FN, immediate commands, etc.)
- Verifying statement parsing in REPL context
- Debugging REPL-specific bugs

#### Key Differences: REPL vs Batch Mode

| Aspect | Batch Mode | REPL Mode |
|--------|------------|-----------|
| Entry Point | `Interpreter.FromText()` | `RuntimeInterpreter.ProcessImmediate()` |
| Parsing Environment | Set by `InterpretProgram()` | Must be set explicitly in `ProcessImmediate()` |
| Statement Processing | `ProcessBlock()` (expects complete blocks) | `ProcessLineForREPL()` (allows incremental entry) |
| FOR-NEXT Loops | Must be complete before parsing | Can be entered line-by-line |
| Error Handling | Collects errors for reporting | Throws immediately |

#### REPL Integration Test Pattern

```csharp
using ECMABasic55;
using ECMABasic.Infrastructure;
using Xunit;

public class ReplIntegrationTests
{
    private readonly RuntimeInterpreter _interpreter;
    private readonly TestEnvironment _env;

    public ReplIntegrationTests()
    {
        _interpreter = new RuntimeInterpreter();
        _env = new TestEnvironment(_interpreter);
    }

    /// <summary>
    /// Simulates typing a line in the REPL.
    /// This is the EXACT code path used by the console REPL.
    /// </summary>
    private void TypeLine(string line)
    {
        // Add newline like the REPL does (see Program.cs line 226)
        var lineWithNewline = line + Environment.NewLine;

        // Call ProcessImmediate - this is what the REPL actually calls
        var statement = _interpreter.ProcessImmediate(_env, lineWithNewline);

        // If a statement was returned, execute it (for immediate commands)
        if (statement != null)
        {
            statement.Execute(_env, true);
        }
    }

    private string Run()
    {
        _env.Program.Execute(_env);
        return _env.Text;
    }

    [Fact]
    public void Repl_DefFn_WorksInInteractiveMode()
    {
        // Build program line by line, just like a user would
        TypeLine("10 LET P=3.14159");
        TypeLine("20 DEF FND(X)=P*X/180");
        TypeLine("30 PRINT FND(90)");
        TypeLine("40 END");

        var result = Run();
        Assert.Contains("1.57", result);
    }
}
```

#### Common REPL Testing Pitfalls

**❌ DON'T: Use `Interpreter.FromText()` for REPL features**
```csharp
// This may pass even when REPL fails!
var program = "10 DEF FND(X)=P*X/180\n20 END\n";
Interpreter.FromText(program, env);  // Wrong - bypasses REPL path
```

**✅ DO: Use `RuntimeInterpreter.ProcessImmediate()`**
```csharp
// This tests the actual REPL code path
var interpreter = new RuntimeInterpreter();
TypeLine("10 DEF FND(X)=P*X/180");  // Correct - uses REPL path
```

#### Test Project Configuration

REPL integration tests require a reference to the ECMABasic55 project:

```xml
<!-- In test/ECMABasic.Test/ECMABasic.Test.csproj -->
<ItemGroup>
  <ProjectReference Include="..\..\src\ECMABasic55\ECMABasic55.csproj" />
</ItemGroup>
```

**Reference**: See `test/ECMABasic.Test/ReplIntegrationTests.cs` for complete examples.

## xUnit Best Practices

### Attributes
- `[Fact]`: Simple test with no parameters
- `[Theory]`: Parameterized test with `[InlineData]`
- `[Skip("reason")]`: Temporarily disable tests (use sparingly)

### Assertions
- Use specific assertions: `Assert.Equal`, `Assert.True`, `Assert.Throws`
- Provide meaningful failure messages
- Assert one concept per test

### Test Isolation
- Each test should be independent
- Don't rely on test execution order
- Clean up resources in Dispose if needed

## Continuous Testing

### During Development
```bash
# Run tests in watch mode
dotnet watch test --project test/ECMABasic.Test
```

### Pre-Commit
- All tests must pass before committing
- Coverage should not decrease
- No skipped tests without documented reason
