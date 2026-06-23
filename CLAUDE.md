# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A .NET 6.0 implementation of the ECMA-55 (Minimal BASIC) and ECMA-116 BASIC standards. This project provides an 80's-style BASIC interpreter with an interactive REPL environment for educational purposes.

**Repository**: https://github.com/treytomes/ecma_basic

## Solution Structure

The solution consists of three projects:

- **ECMABasic.Core**: Core interpreter library containing the parser, expression evaluator, and statement implementations. Includes configuration system for BASIC dialect settings.
- **ECMABasic55**: Console application (ecmabasic55.exe) that provides the interactive REPL. Extends the core interpreter with immediate-mode commands (RUN, LIST, LOAD, SAVE, etc.).
- **ECMABasic.Test**: xUnit test suite with sample programs in `Resources/` directory. Tests are organized by ECMA-55 standard groups (Group1-7).

## Development Commands

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

# Run specific test project
dotnet test src/ECMABasic.Test/ECMABasic.Test.csproj

# Run a specific test class
dotnet test --filter Group1SampleTests

# Run a single test method
dotnet test --filter "FullyQualifiedName~P050_Hello"
```

Test resources (sample BASIC programs) are in `src/ECMABasic.Test/Resources/` with `.BAS` source files and `.OK` expected output files.

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

Tests follow xUnit conventions with test classes organized by ECMA-55 standard sections:

- Test methods named by test case number (e.g., `P050_Hello()`, `P055_Goodbye()`)
- Sample programs stored in `Resources/` as `.BAS` files
- Expected output stored as `.OK` files with matching names
- Test resources copied to output directory via project configuration

Common test pattern:
```csharp
[Fact]
public void P050_Hello()
{
    var result = RunProgram("P050-HELLO.BAS");
    Assert.Equal(expectedOutput, result);
}
```
