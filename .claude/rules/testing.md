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
