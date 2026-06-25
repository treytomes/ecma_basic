# PRINT Statement Comprehensive Test Specification

## Issue Reference
Related to ECMA-55 conformance testing and Issue #37 follow-up

## Overview
The PRINT statement is one of the most fundamental and complex statements in BASIC. It handles numeric formatting, string output, zone positioning, TAB functions, and multiple separator types. This specification ensures comprehensive test coverage of all ECMA-55 requirements for PRINT.

## ECMA-55 Requirements Summary

### Core Requirements

| Requirement | Description | Current Status |
|------------|-------------|----------------|
| **ECMA55-PRN-001** | PRINT items can be expressions, TAB(...) calls, or null items separated by commas/semicolons | ✓ Implemented |
| **ECMA55-PRN-005** | Exact integers with ≤d decimal digits print in implicit-point form (no decimal point) | ✓ Implemented |
| **ECMA55-PRN-006** | Other numbers print in explicit-point or scaled form per accuracy rules | ✓ Implemented |
| **ECMA55-PRN-008** | Implementation defines output margin, number of zones, zone length | ✓ Documented |
| **ECMA55-PRN-009** | Comma advances to next print zone, or EOL if in last zone | ✓ Implemented |
| **ECMA55-PRN-010** | TAB(n) moves to column n, wraps by margin if n > margin | ✓ Implemented |
| **ECMA55-PRN-012** | PRINT without trailing separator generates EOL | ✓ Implemented |
| **ECMA55-PRN-013** | Item exceeding margin causes EOL before item | ✓ Implemented |
| **ECMA55-PRN-014** | Single item > margin has EOL every margin chars | ✓ Implemented |

### Configuration Parameters (from appsettings.json)

- **terminalWidth**: 80 columns (output margin)
- **numTerminalColumns**: 5 print zones
- **Print zone width**: 80 / 5 = 16 columns per zone
- **significanceWidth**: 6 digits (numeric precision)
- **exradWidth**: 2 digits (exponent width)

## Test Categories

### 1. Basic Output Tests

#### 1.1 Empty PRINT
```basic
10 PRINT
```
**Expected**: Single blank line
**Requirement**: ECMA55-PRN-012

#### 1.2 Single String
```basic
10 PRINT "HELLO"
```
**Expected**: `HELLO` followed by newline
**Requirement**: ECMA55-PRN-001, PRN-012

#### 1.3 Single Number (Integer)
```basic
10 PRINT 42
```
**Expected**: ` 42 ` (with leading and trailing spaces)
**Requirement**: ECMA55-PRN-005

#### 1.4 Single Number (Decimal)
```basic
10 PRINT 3.14159
```
**Expected**: ` 3.14159 ` (formatted per significance-width)
**Requirement**: ECMA55-PRN-006

#### 1.5 Multiple Items with Semicolons
```basic
10 PRINT "A";"B";"C"
```
**Expected**: `ABC` (no spaces between)
**Requirement**: ECMA55-PRN-001

#### 1.6 Multiple Items with Commas
```basic
10 PRINT "A","B","C"
```
**Expected**: `A` at col 0, `B` at col 16, `C` at col 32 (zone boundaries)
**Requirement**: ECMA55-PRN-009

### 2. Separator Tests

#### 2.1 Trailing Semicolon (Suppress EOL)
```basic
10 PRINT "HELLO";
20 PRINT "WORLD"
```
**Expected**: `HELLOWORLD` on one line
**Requirement**: ECMA55-PRN-012

#### 2.2 Trailing Comma (Next Zone)
```basic
10 PRINT "A",
20 PRINT "B"
```
**Expected**: `A` in zone 1, `B` in zone 2 on same line
**Requirement**: ECMA55-PRN-009, PRN-012

#### 2.3 Multiple Commas (Zone Advancement)
```basic
10 PRINT "A",,"C"
```
**Expected**: `A` in zone 1, null in zone 2, `C` in zone 3
**Requirement**: ECMA55-PRN-001, PRN-009

#### 2.4 Comma in Last Zone (Wrap to Next Line)
```basic
10 PRINT "A","B","C","D","E",
20 PRINT "F"
```
**Expected**: A-E in zones 1-5 on line 1, F in zone 1 on line 2
**Requirement**: ECMA55-PRN-009

### 3. Numeric Formatting Tests

#### 3.1 Zero
```basic
10 PRINT 0
```
**Expected**: ` 0 `
**Requirement**: ECMA55-PRN-005

#### 3.2 Positive Integer
```basic
10 PRINT 123
```
**Expected**: ` 123 ` (space before positive numbers)
**Requirement**: ECMA55-PRN-005

#### 3.3 Negative Integer
```basic
10 PRINT -456
```
**Expected**: `-456 ` (minus sign, no leading space)
**Requirement**: ECMA55-PRN-005

#### 3.4 Small Decimal
```basic
10 PRINT 0.123456
```
**Expected**: ` .123456 ` (implicit point form)
**Requirement**: ECMA55-PRN-006

#### 3.5 Large Number (Scientific Notation)
```basic
10 PRINT 1234567890
```
**Expected**: ` 1.23457E+09 ` (or similar based on significance-width)
**Requirement**: ECMA55-PRN-006

#### 3.6 Very Small Number (Scientific Notation)
```basic
10 PRINT 0.000000123
```
**Expected**: ` 1.23E-07 ` (scaled form)
**Requirement**: ECMA55-PRN-006

#### 3.7 Special Values
```basic
10 LET A = 1 / 0
20 PRINT A
30 LET B = 0 / 0
40 PRINT B
50 LET C = -1 / 0
60 PRINT C
```
**Expected**: ` INF `, ` NAN `, `-INF `
**Requirement**: ECMA55-PRN-006 (implementation-defined)

### 4. TAB Function Tests

#### 4.1 TAB to Column
```basic
10 PRINT "A";TAB(10);"B"
```
**Expected**: `A` at col 0, `B` at col 10
**Requirement**: ECMA55-PRN-010

#### 4.2 TAB with Comma
```basic
10 PRINT "A",TAB(20),"B"
```
**Expected**: `A` in zone 1, spaces to col 20, `B` at col 20
**Requirement**: ECMA55-PRN-010

#### 4.3 TAB Beyond Current Position
```basic
10 PRINT "HELLO";TAB(10);"WORLD"
```
**Expected**: `HELLO` (0-4), spaces to col 10, `WORLD` at 10
**Requirement**: ECMA55-PRN-010

#### 4.4 TAB Before Current Position (Wrap)
```basic
10 PRINT "HELLO WORLD";TAB(5);"X"
```
**Expected**: `HELLO WORLD` on line 1, `X` at col 5 on line 2
**Requirement**: ECMA55-PRN-010

#### 4.5 TAB Greater Than Margin (Modulo Wrapping)
```basic
10 PRINT TAB(90);"X"
```
**Expected**: `X` at column 90 mod 80 = 10
**Requirement**: ECMA55-PRN-010

#### 4.6 TAB with Expression
```basic
10 LET N = 15
20 PRINT TAB(N * 2);"X"
```
**Expected**: `X` at column 30
**Requirement**: ECMA55-PRN-010

### 5. Margin and Wrapping Tests

#### 5.1 Item Exceeding Margin
```basic
10 PRINT "THIS IS A VERY LONG STRING THAT EXCEEDS THE MARGIN WIDTH"
```
**Expected**: EOL after 80 chars, continuation on next line
**Requirement**: ECMA55-PRN-013

#### 5.2 Multiple Items Exceeding Margin
```basic
10 PRINT "ITEM1";"ITEM2";"ITEM3";"ITEM4";"ITEM5";"ITEM6"
```
**Expected**: Items wrap at margin boundary
**Requirement**: ECMA55-PRN-013

#### 5.3 Single Item > Margin (Forced Breaks)
```basic
10 PRINT "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
```
**Expected**: 80 A's on line 1, remaining A's on line 2
**Requirement**: ECMA55-PRN-014

### 6. Expression Tests

#### 6.1 Arithmetic Expression
```basic
10 PRINT 2 + 3
```
**Expected**: ` 5 `
**Requirement**: ECMA55-PRN-001

#### 6.2 Variable
```basic
10 LET A = 42
20 PRINT A
```
**Expected**: ` 42 `
**Requirement**: ECMA55-PRN-001

#### 6.3 Function Call
```basic
10 PRINT SQR(16)
```
**Expected**: ` 4 `
**Requirement**: ECMA55-PRN-001

#### 6.4 String Expression
```basic
10 PRINT "HELLO" + " " + "WORLD"
```
**Expected**: `HELLO WORLD`
**Requirement**: ECMA55-PRN-001

#### 6.5 Boolean Expression Result
```basic
10 PRINT 5 > 3
```
**Expected**: `-1 ` (TRUE is -1 in BASIC)
**Requirement**: ECMA55-PRN-001

### 7. Mixed Content Tests

#### 7.1 String and Number with Semicolon
```basic
10 PRINT "VALUE:";42
```
**Expected**: `VALUE: 42 `
**Requirement**: ECMA55-PRN-001

#### 7.2 String and Number with Comma
```basic
10 PRINT "NAME","AGE"
20 PRINT "ALICE",30
```
**Expected**: Columnar output in zones
**Requirement**: ECMA55-PRN-009

#### 7.3 Multiple Data Types
```basic
10 PRINT "TEXT";123;"MORE";4.56,"ZONE"
```
**Expected**: Mixed formatting with proper separators
**Requirement**: ECMA55-PRN-001

### 8. Edge Cases

#### 8.1 Empty String
```basic
10 PRINT ""
```
**Expected**: Blank line (EOL only)
**Requirement**: ECMA55-PRN-001

#### 8.2 Just Semicolon
```basic
10 PRINT ;
```
**Expected**: No output, no EOL
**Requirement**: ECMA55-PRN-012

#### 8.3 Just Comma
```basic
10 PRINT ,
```
**Expected**: Advance to zone 2, no EOL
**Requirement**: ECMA55-PRN-009

#### 8.4 Multiple Consecutive Separators
```basic
10 PRINT "A";;;,"B"
```
**Expected**: `A` no spaces, then `B` in next zone
**Requirement**: ECMA55-PRN-001

#### 8.5 TAB(0)
```basic
10 PRINT TAB(0);"X"
```
**Expected**: `X` at column 0
**Requirement**: ECMA55-PRN-010

#### 8.6 TAB(1)
```basic
10 PRINT TAB(1);"X"
```
**Expected**: `X` at column 1
**Requirement**: ECMA55-PRN-010

#### 8.7 Negative TAB
```basic
10 PRINT TAB(-5);"X"
```
**Expected**: Error or wrap behavior (implementation-defined)
**Requirement**: ECMA55-PRN-010

### 9. Interactive Mode Tests

#### 9.1 PRINT in Interactive Mode
```
PRINT "HELLO"
```
**Expected**: Immediate output, no line number needed
**Requirement**: General interpreter requirement

#### 9.2 PRINT with Trailing Semicolon in Interactive
```
PRINT "A";
PRINT "B"
```
**Expected**: `AB` on same line
**Requirement**: ECMA55-PRN-012

### 10. Numeric Formatting Edge Cases

#### 10.1 Significance Width Boundary
```basic
10 PRINT 123456
20 PRINT 1234567
```
**Expected**: First as integer, second may be scientific
**Requirement**: ECMA55-PRN-005, PRN-006

#### 10.2 Rounding
```basic
10 PRINT 1.2345678901234
```
**Expected**: Rounded to significance-width (6 digits): ` 1.23457 `
**Requirement**: ECMA55-PRN-006

#### 10.3 Very Close to Zero
```basic
10 PRINT 0.0000001
```
**Expected**: ` 1.E-07 ` or similar
**Requirement**: ECMA55-PRN-006

## Test Implementation Plan

### Phase 1: Core Functionality (High Priority)
- Empty PRINT
- Single string/number output
- Semicolon and comma separators
- Trailing separator behavior
- Basic numeric formatting

### Phase 2: TAB Function (Medium Priority)
- TAB positioning
- TAB with expressions
- TAB wrapping
- TAB edge cases

### Phase 3: Advanced Formatting (Medium Priority)
- Scientific notation
- Special values (INF, NAN)
- Significance width
- Zone positioning

### Phase 4: Edge Cases (Low Priority)
- Margin wrapping
- Long strings
- Multiple consecutive separators
- Negative TAB values

## Test File Structure

Create new test file: `test/ECMABasic.Test/PrintStatementTests.cs`

```csharp
namespace ECMABasic.Test;

/// <summary>
/// Tests for PRINT statement per ECMA-55 Section 14.
/// ECMA55-PRN-001 through PRN-014: Print statement formatting and output.
/// </summary>
public class PrintStatementTests
{
    #region Helper Methods
    
    private string RunProgram(string program)
    {
        var env = new TestEnvironment();
        Interpreter.FromText(program, env);
        env.Program.Execute(env);
        return env.Text;
    }
    
    #endregion
    
    #region Basic Output Tests (ECMA55-PRN-001, PRN-012)
    // Tests here
    #endregion
    
    #region Separator Tests (ECMA55-PRN-009, PRN-012)
    // Tests here
    #endregion
    
    #region Numeric Formatting Tests (ECMA55-PRN-005, PRN-006)
    // Tests here
    #endregion
    
    #region TAB Function Tests (ECMA55-PRN-010)
    // Tests here
    #endregion
    
    #region Margin and Wrapping Tests (ECMA55-PRN-013, PRN-014)
    // Tests here
    #endregion
    
    #region Expression Tests (ECMA55-PRN-001)
    // Tests here
    #endregion
    
    #region Edge Cases
    // Tests here
    #endregion
}
```

## Acceptance Criteria

- [ ] All ECMA55-PRN requirements have corresponding tests
- [ ] Test coverage for PRINT statement ≥ 90%
- [ ] All test categories implemented
- [ ] Edge cases validated
- [ ] Numeric formatting tested across full range
- [ ] TAB function fully tested
- [ ] Zone positioning validated
- [ ] Margin wrapping tested
- [ ] All tests pass
- [ ] Zero compiler warnings

## Success Metrics

- **Minimum 50 unit tests** for PRINT statement
- **100% of ECMA-55 PRN requirements** covered
- **All tests passing**
- **Code coverage ≥ 90%** for PrintStatement.cs and PrintStatementParser.cs

## Related Files

- **Implementation**: `src/ECMABasic.Application/Statements/PrintStatement.cs`
- **Parser**: `src/ECMABasic.Application/Parsers/PrintStatementParser.cs`
- **Tests**: `test/ECMABasic.Test/PrintStatementTests.cs` (to be created)
- **Config**: `src/ECMABasic55/appsettings.json` (terminalWidth, numTerminalColumns, etc.)

## References

- ECMA-55 Standard Section 14: Print Statement
- Current test files for reference patterns
- Issue #37 (DEF FN) as implementation quality benchmark
