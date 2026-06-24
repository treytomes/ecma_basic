# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Project Overview

A .NET 10 implementation of the ECMA-55 (Minimal BASIC) and ECMA-116 BASIC standards. This project provides an 80's-style BASIC interpreter with an interactive REPL environment for educational purposes.

**Repository**: https://github.com/treytomes/ecma_basic  
**Wiki**: https://github.com/treytomes/ecma_basic/wiki/  
**Issues**: https://github.com/treytomes/ecma_basic/issues

## Development Philosophy

### Spec-Driven Development

**CRITICAL: Never modify source code without a specification.**

Every code change requires ONE of:
1. **Failing test** that defines expected behavior (written first)
2. **GitHub issue** with acceptance criteria
3. **ECMA standard reference** with corresponding test
4. **Pure refactoring** (no behavior change, all tests pass)

#### Process
- **Specifications are GitHub Issues**: Feature requests and bugs are tracked as issues with acceptance criteria
- **Tests as Living Specs**: Every feature has tests that serve as executable specifications
- **Red-Green-Refactor**: Write failing tests first, implement to pass, then refactor
- **Documentation in Wiki**: Project notes, architecture decisions, and guides live in the GitHub wiki

If asked to modify code without a spec, STOP and request either a GitHub issue or write the test first.

### Security and Secrets Management

**CRITICAL: Never commit secrets, tokens, or API keys to git.**

- ✅ **Safe to commit**: `.Codex/settings.json` (no secrets)
- 🔒 **Never commit**: `.Codex/settings.local.json` (gitignored, contains tokens)

**Token Storage Options**:
1. `.Codex/settings.local.json` (recommended for development)
2. System environment variable `GITHUB_TOKEN`
3. GitHub CLI credential store: `gh auth token`

See [`.Codex/SECRETS_MANAGEMENT.md`](.Codex/SECRETS_MANAGEMENT.md) for complete security guide.

### Clean Architecture
- **Core Domain**: The `ECMABasic.Application` project contains domain logic with no external dependencies
- **Application Layer**: `ECMABasic55` provides the console REPL, depending only on Core
- **Dependency Rule**: Dependencies point inward (toward Core), never outward
- **Interface-Driven**: Use abstractions (`IEnvironment`, `IBasicConfiguration`) for extensibility

### Quality Standards
- **80% Code Coverage Minimum**: All production code must maintain 80%+ test coverage
- **Nullable Reference Types**: Explicit null handling throughout the codebase
- **Modern C# Patterns**: File-scoped namespaces, target-typed new, pattern matching, var keyword, etc.
- **Zero Warnings**: Build treats ALL warnings as errors (enforced via `Directory.Build.props`)
  - The build WILL FAIL if any warnings exist
  - This includes nullable warnings, code style warnings, and XML documentation warnings
  - New projects automatically inherit this configuration

## Solution Structure

The solution follows Clean Architecture principles with three projects:

- **ECMABasic.Application**: Core domain layer containing the interpreter, parser, expression evaluator, and statement implementations. No external dependencies. Includes configuration system for BASIC dialect settings.
- **ECMABasic55**: Application layer - console executable (ecmabasic55.exe) providing the interactive REPL. Extends the core interpreter with immediate-mode commands (RUN, LIST, LOAD, SAVE, etc.). Depends only on Core.
- **ECMABasic.Test**: Test layer using xUnit with sample programs in `Resources/` directory. Tests organized by ECMA-55 standard groups (Group1-7). Validates specifications and maintains 80%+ coverage.

## Development Commands

### Quick Start Scripts

Convenience scripts are provided in the root directory for common tasks:

```bash
# Build the solution (Bash or Batch)
./build.sh      # or build.bat on Windows

# Run tests with coverage
./test.sh       # or test.bat on Windows

# Run the BASIC interpreter
./run.sh        # or run.bat on Windows

# Run with a BASIC program file
./run.sh path/to/program.BAS

# Publish for all platforms
./publish.sh    # or publish.bat on Windows
```

### Building and Running

```bash
# Build the entire solution
dotnet build src/ECMABasic.sln

# Run the BASIC interpreter (interactive REPL)
dotnet run --project src/ECMABasic55/ECMABasic55.csproj

# Run with a BASIC program file
dotnet run --project src/ECMABasic55/ECMABasic55.csproj -- path/to/program.BAS
```

### Testing

```bash
# Run all tests
dotnet test src/ECMABasic.sln

# Run with code coverage (minimum 80% required)
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test src/ECMABasic.Test/ECMABasic.Test.csproj

# Run a specific test class
dotnet test --filter Group1SampleTests

# Run a single test method
dotnet test --filter "FullyQualifiedName~P050_Hello"

# Watch mode for continuous testing during development
dotnet watch test --project src/ECMABasic.Test
```

Test resources (sample BASIC programs) are in `src/ECMABasic.Test/Resources/` with `.BAS` source files and `.OK` expected output files.

### Code Coverage

```bash
# Generate coverage report
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# View report (Windows)
start coverage-report/index.html
```

**Minimum 80% code coverage is required.** Focus on meaningful tests of business logic, parsers, and interpreters rather than trivial getters/setters.

## Architecture

### Core Interpreter Pipeline

The interpreter follows a multi-stage pipeline:

1. **CharacterReader**: Low-level character stream reader
2. **ComplexTokenReader**: Tokenizes input into semantic units (keywords, identifiers, operators, literals)
3. **Interpreter**: Parses tokens into an Abstract Syntax Tree (AST) using statement parsers
4. **Program**: Executes the AST through the IEnvironment interface

### Key Components

**Interpreter** (`Interpreter.cs`): 
- Registry of StatementParser instances that convert tokens to IStatement nodes
- Core statements include: LET, PRINT, GOTO, GOSUB, RETURN, IF-THEN, FOR-NEXT, READ-DATA, INPUT
- Extensible via `InjectStatements()` to add custom statement types

**RuntimeInterpreter** (`RuntimeInterpreter.cs`):
- Extends base Interpreter with immediate-mode commands (RUN, LIST, NEW, LOAD, SAVE, CONTINUE)
- Handles both line-numbered program entry and direct command execution

**IEnvironment** (`IEnvironment.cs`):
- Runtime execution context managing variables, I/O, call stack, and program state
- Implementations provide different execution environments (console, testing, etc.)

**StatementParser Pattern** (`Parsers/` directory):
- Each BASIC statement has a dedicated parser class (e.g., `LetStatementParser`, `PrintStatementParser`)
- Parsers consume tokens from ComplexTokenReader and return IStatement implementations

**Expression System** (`Expressions/` directory):
- Recursive expression tree for arithmetic and boolean operations
- Operator precedence: involution (^) > unary negation > multiplication/division > addition/subtraction > comparisons

### Configuration System

BASIC dialect behavior is controlled via `IBasicConfiguration`:

- **maxLineLength**: Maximum characters per program line (default: 72)
- **maxStringLength**: Maximum string variable length (default: 18)
- **terminalWidth**: Console width for formatting (default: 80)
- **numTerminalColumns**: Number of PRINT zone columns (default: 5)
- **maxLineNumberDigits**: Line number range constraint (default: 4 = max 9999)
- **significanceWidth/exradWidth**: Numeric formatting precision

Configuration loaded from `appsettings.json` in ECMABasic55 project.

## BASIC Language Features

### Immediate Mode Commands

Available at the REPL prompt without line numbers:

- `RUN`: Execute the program in memory
- `LIST`: Display the current program
- `NEW`: Clear the program from memory
- `LOAD "filename"`: Load a program from disk
- `SAVE "filename"`: Save the current program to disk
- `CONTINUE`: Resume after STOP statement

### Program Mode Statements

Line-numbered statements that comprise a BASIC program:

- `LET variable = expression`: Variable assignment (LET keyword optional)
- `PRINT expression[, expression...]`: Output to console
- `INPUT variable[, variable...]`: Read user input
- `GOTO line`: Jump to line number
- `GOSUB line` / `RETURN`: Subroutine call/return
- `IF condition THEN line`: Conditional branch
- `ON expression GOTO line[, line...]`: Computed GOTO
- `FOR variable = start TO end [STEP increment]` / `NEXT variable`: Loops
- `READ variable[, variable...]`: Read from DATA statements
- `DATA value[, value...]`: Define data constants
- `RESTORE`: Reset DATA pointer to beginning
- `REM comment`: Comment line
- `STOP`: Halt execution (resumable with CONTINUE)
- `END`: Terminate program

### Variable Naming

- Numeric variables: Single letter or letter + digit (e.g., `A`, `X1`, `Z9`)
- String variables: Same as numeric but ending with `$` (e.g., `A$`, `N1$`)

## Testing Patterns

### Test Organization
Tests follow xUnit conventions organized by ECMA-55 standard sections with **80% minimum code coverage requirement**:

- Test methods named by test case number (e.g., `P050_Hello()`, `P055_Goodbye()`)
- Sample programs stored in `Resources/` as `.BAS` files
- Expected output stored as `.OK` files with matching names
- Test resources copied to output directory via project configuration

### Common Test Pattern
```csharp
[Fact]
public void P050_Hello()
{
    // Arrange-Act-Assert pattern
    var result = RunProgram("P050-HELLO.BAS");
    var expected = LoadExpectedOutput("P050-HELLO.OK");
    
    // Assert
    Assert.Equal(expected, result);
}
```

### Test-Driven Development
Follow the Red-Green-Refactor cycle:
1. **Red**: Write a failing test that defines desired behavior (create GitHub issue for the spec)
2. **Green**: Implement minimum code to make the test pass
3. **Refactor**: Improve code quality while keeping tests passing

### GitHub Integration
- **Specifications**: Document features and requirements as GitHub issues with acceptance criteria
- **Documentation**: Architecture decisions and project notes go in the GitHub wiki
- **Test Traceability**: Reference issue numbers in test documentation

## Git Conventions

### Commit Message Format

All commits MUST follow [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>[optional scope]: <description>

[optional body]

Co-Authored-By: Codex Sonnet 4.5 <noreply@anthropic.com>
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`

**Examples**:
```
feat(parser): add MID$ function support
fix(interpreter): handle null in FOR statement  
docs: update README with modernization status
test: add coverage for nullable edge cases
refactor(core): convert to file-scoped namespaces
```

**Rules**:
- Use imperative mood: "add" not "added"
- Lowercase description, no period at end
- Subject line ≤ 72 characters
- Body explains WHY, not WHAT
- Always include Co-Authored-By footer for Codex commits

See `.Codex/rules/git-conventions.md` for complete specification.

### Branch Naming

```
<type>/<issue-number>-<short-description>

Examples:
feature/1-fix-nullable-warnings
fix/42-null-reference-in-for-statement
refactor/3-file-scoped-namespaces
docs/update-readme
```
