# ECMA-55 Derived Requirements

This file restates the normative content of ECMA-55 as repository-friendly requirements.

## Core Scope And Glossary

### Definitions

- `Minimal BASIC`: Programs conforming to ECMA-55 without extensions.
- `Fatal exception`: An exception that terminates the program.
- `Nonfatal exception`: An exception with a recommended recovery procedure.
- `Implementation-defined`: Behavior that must be defined by the implementation manual.

### Scope Requirements

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-GEN-001 | The specification set for Minimal BASIC shall define syntax, accepted input formats, generated output formats, semantics, and required error or exception handling. | Manual | SS1, PDF p. 6 |
| ECMA55-GEN-002 | A program shall be called Minimal BASIC only when it conforms to ECMA-55 rather than relying on extensions or enhancements. | Manual | SS1, PDF p. 6 |
| ECMA55-GEN-003 | Interactive use shall not be assumed as the only supported execution model; the language model shall also support non-interactive execution. | Runtime | SS1, PDF p. 6 |

## Section 4: Characters And Strings

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-CHR-001 | Letters shall be the uppercase Roman letters from the ECMA 7-bit coded character set. | Parser | SS4.4, PDF p. 9 |
| ECMA55-CHR-002 | Digits shall be the Arabic digits `0` through `9` from the ECMA 7-bit coded character set. | Parser | SS4.4, PDF p. 9 |
| ECMA55-CHR-003 | Only the string characters defined by Section 4 shall have prescribed meaning in standard-conforming programs. | Parser | SS4.4-4.6, PDF pp. 8-9 |
| ECMA55-CHR-004 | Quoted strings shall begin and end with quotation marks and may contain spaces and commas internally. | Parser | SS4.2-4.4, PDF pp. 8-9 |
| ECMA55-CHR-005 | Unquoted strings shall be composed only of plain-string characters and shall not contain commas. | Parser | SS4.2-4.6, PDF pp. 8-9 |
| ECMA55-CHR-006 | Programs containing characters outside the defined string-character set shall be treated as non-conforming. | Parser | SS4.6, PDF p. 9 |
| ECMA55-CHR-007 | Character coding in Table 2 shall apply only when programs or data are exchanged through coded media. | Manual | SS4.4, PDF p. 9 |

## Section 5: Programs

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-PRG-001 | A BASIC program shall be line-oriented. | Parser | SS5.1-5.4, PDF pp. 10-11 |
| ECMA55-PRG-002 | A program shall consist of a sequence of lines ordered by ascending line number. | Parser | SS5.4, PDF pp. 10-11 |
| ECMA55-PRG-003 | Each program line shall begin with a unique positive nonzero line number. | Parser | SS5.1-5.4, PDF pp. 10-11 |
| ECMA55-PRG-004 | Leading zeroes in a line number shall not change its numeric value. | Parser | SS5.6, PDF p. 11 |
| ECMA55-PRG-005 | Each program line shall contain a keyword. | Parser | SS5.1, PDF p. 10 |
| ECMA55-PRG-006 | The last program line shall contain an `END` statement. | Parser | SS5.1-5.4, PDF pp. 10-11 |
| ECMA55-PRG-007 | Program execution shall begin at the first line and continue sequentially unless redirected by control flow, terminated by exception, or terminated by `STOP` or `END`. | Runtime | SS5.4, PDF p. 10 |
| ECMA55-PRG-008 | Spaces may appear anywhere without changing program meaning except where Section 5 explicitly forbids them. | Parser | SS5.4, PDF p. 10 |
| ECMA55-PRG-009 | Spaces shall not appear at the beginning of a line. | Parser | SS5.4, PDF p. 10 |
| ECMA55-PRG-010 | Spaces shall not appear within keywords, numeric constants, line numbers, function names, variable names, or two-character relation symbols. | Parser | SS5.4, PDF p. 10 |
| ECMA55-PRG-011 | Every keyword shall be preceded by at least one space and, unless it ends the line, followed by at least one space. | Parser | SS5.6, PDF p. 11 |
| ECMA55-PRG-012 | A standard-conforming program line shall contain no more than 72 characters excluding the end-of-line indicator. | Parser | SS5.6, PDF p. 11 |
| ECMA55-PRG-013 | Detection of end-of-line shall be implementation-defined. | Documentation | SS5.6, PDF p. 11 |

## Section 6: Constants

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-CON-001 | Numeric constants shall support implicit-point, explicit-point unscaled, explicit-point scaled, and implicit-point scaled decimal forms. | Parser | SS6.1-6.2, PDF pp. 11-12 |
| ECMA55-CON-002 | A numeric constant may have an optional leading sign. | Parser | SS6.1-6.2, PDF pp. 11-12 |
| ECMA55-CON-003 | Spaces shall not occur within numeric constants. | Parser | SS6.4, PDF p. 12 |
| ECMA55-CON-004 | Implementations may round numeric representations, but accepted precision shall be at least six significant decimal digits. | Runtime | SS6.4, PDF p. 12 |
| ECMA55-CON-005 | The implementation-defined numeric range shall be at least `1E-38` through `1E+38` for nonzero magnitudes. | Runtime | SS6.4, PDF p. 12 |
| ECMA55-CON-006 | Numeric constants smaller than machine infinitesimal shall be replaced by zero. | Runtime | SS6.4, PDF p. 12 |
| ECMA55-CON-007 | Numeric constants larger than machine infinity shall raise a nonfatal overflow exception, with recommended recovery by supplying machine infinity with the appropriate sign and continuing. | Runtime | SS6.5, PDF p. 12 |
| ECMA55-CON-008 | A string constant shall evaluate to the exact characters between its quotation marks, including internal spaces. | Runtime | SS6.4, PDF p. 12 |
| ECMA55-CON-009 | The length of a string constant shall be limited only by line length. | Parser | SS6.4, PDF p. 12 |

## Section 7: Variables

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-VAR-001 | A simple numeric variable name shall be a letter followed by an optional digit. | Parser | SS7.1-7.4, PDF p. 13 |
| ECMA55-VAR-002 | A string variable name shall be a letter followed by `$`. | Parser | SS7.1-7.4, PDF p. 13 |
| ECMA55-VAR-003 | A numeric array element reference shall use a letter followed by one or two numeric subscripts in parentheses. | Parser | SS7.1-7.4, PDF p. 13 |
| ECMA55-VAR-004 | Numeric variables shall hold numeric values and string variables shall hold string values. | Runtime | SS7.4, PDF p. 13 |
| ECMA55-VAR-005 | The string value associated with a string variable shall be allowed to vary from zero to 18 characters in length. | Runtime | SS7.5, PDF p. 14 |
| ECMA55-VAR-006 | Variables shall be implicitly declared by use. | Parser | SS7.5, PDF pp. 13-14 |
| ECMA55-VAR-007 | Subscript values shall be rounded to the nearest integer before array lookup. | Runtime | SS7.5, PDF p. 14 |
| ECMA55-VAR-008 | If no explicit dimension statement applies, each array subscript range shall default to `0..10`, or `1..10` when `OPTION BASE 1` is in effect. | Runtime | SS7.5, PDF p. 14 |
| ECMA55-VAR-009 | The same letter shall not name both a simple variable and an array, nor both a one-dimensional and a two-dimensional array. | Parser | SS7.5, PDF p. 14 |
| ECMA55-VAR-010 | A numeric variable and a string variable that differ only by `$` shall be treated as unrelated names. | Runtime | SS7.5, PDF p. 14 |
| ECMA55-VAR-011 | Initial variable values shall be implementation-defined. | Documentation | SS7.5, PDF p. 14 |
| ECMA55-VAR-012 | A subscript outside the explicit or implicit array bounds shall raise a fatal exception. | Runtime | SS7.6, PDF p. 14 |

## Section 8: Expressions

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-EXP-001 | An expression shall be either numeric or string. | Parser | SS8.1-8.2, PDF pp. 14-15 |
| ECMA55-EXP-002 | Numeric expressions shall be formed from variables, numeric constants, function references, and parenthesized numeric expressions using involution, multiplication, division, addition, and subtraction. | Parser | SS8.1-8.2, PDF pp. 14-15 |
| ECMA55-EXP-003 | String expressions shall be limited to string variables and string constants. | Parser | SS8.1-8.2, PDF pp. 14-15 |
| ECMA55-EXP-004 | Operator precedence shall be involution first, then multiplication or division, then addition or subtraction, unless parentheses override that order. | Runtime | SS8.4, PDF p. 15 |
| ECMA55-EXP-005 | Operations of the same precedence shall associate to the left when parentheses do not force a different structure. | Runtime | SS8.4, PDF p. 15 |
| ECMA55-EXP-006 | Underflow during numeric-expression evaluation shall replace the result of the underflowing operation with zero. | Runtime | SS8.4, PDF p. 15 |
| ECMA55-EXP-007 | `0^0` shall evaluate to `1`. | Runtime | SS8.4, PDF p. 15 |
| ECMA55-EXP-008 | A function reference shall supply exactly the number of arguments required by the referenced function definition. | Parser | SS8.4, PDF p. 15 |
| ECMA55-EXP-009 | Division by zero shall raise a nonfatal exception, with recommended recovery by supplying machine infinity with the numerator sign and continuing. | Runtime | SS8.5, PDF p. 16 |
| ECMA55-EXP-010 | Numeric overflow during expression evaluation shall raise a nonfatal exception, with recommended recovery by supplying machine infinity with the algebraically correct sign and continuing. | Runtime | SS8.5, PDF p. 16 |
| ECMA55-EXP-011 | Raising a negative number to a non-integral power shall raise a fatal exception. | Runtime | SS8.5, PDF p. 16 |
| ECMA55-EXP-012 | Raising zero to a negative power shall raise a nonfatal exception, with recommended recovery by supplying positive machine infinity and continuing. | Runtime | SS8.5, PDF p. 16 |

## Section 9: Implementation-Supplied Functions

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-FUN-001 | The implementation shall provide `ABS`, `ATN`, `COS`, `EXP`, `INT`, `LOG`, `RND`, `SGN`, `SIN`, `SQR`, and `TAN`. | Parser | SS9.2, PDF p. 16 |
| ECMA55-FUN-002 | Each supplied function shall implement the semantics described in Section 9 for its argument and return value. | Runtime | SS9.4, PDF pp. 17-18 |
| ECMA55-FUN-003 | `EXP(X)` shall return zero if its computed value is less than machine infinitesimal. | Runtime | SS9.4, PDF p. 17 |
| ECMA55-FUN-004 | `RND` shall return the next pseudo-random number from an implementation-supplied sequence uniformly distributed over `0 <= RND < 1`. | Runtime | SS9.4, PDF p. 17 |
| ECMA55-FUN-005 | `LOG(X)` with zero or negative input shall raise a fatal exception. | Runtime | SS9.5, PDF p. 17 |
| ECMA55-FUN-006 | `SQR(X)` with negative input shall raise a fatal exception. | Runtime | SS9.5, PDF p. 17 |
| ECMA55-FUN-007 | `EXP` or `TAN` values whose magnitude exceeds machine infinity shall raise nonfatal exceptions, with recommended recovery by supplying machine infinity with the correct sign and continuing. | Runtime | SS9.5, PDF p. 17 |
| ECMA55-FUN-008 | In the absence of `RANDOMIZE`, repeated runs of the same program shall use the same pseudo-random sequence. | Runtime | SS9.6, PDF p. 18 |

## Section 10: User-Defined Functions

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-DEF-001 | User-defined function declarations shall use `DEF FNx = expression` or `DEF FNx(parameter) = expression`, where `x` is a single letter and the parameter is a simple numeric variable. | Parser | SS10.1-10.2, PDF p. 18 |
| ECMA55-DEF-002 | Referencing a user-defined function shall evaluate the supplied argument expression, assign it to the parameter, then evaluate the defining expression and use that result as the function value. | Runtime | SS10.4, PDF p. 18 |
| ECMA55-DEF-003 | The parameter named in a function definition shall be local to that definition. | Runtime | SS10.5, PDF p. 19 |
| ECMA55-DEF-004 | Variables not named in the parameter list shall refer to the variables of the same name outside the function definition. | Runtime | SS10.5, PDF p. 19 |
| ECMA55-DEF-005 | A function definition shall appear on a lower-numbered line than its first reference. | Parser | SS10.5, PDF p. 19 |
| ECMA55-DEF-006 | Executing a `DEF` line directly shall have no effect other than continuing to the next line. | Runtime | SS10.5, PDF p. 19 |
| ECMA55-DEF-007 | A function definition may reference other defined functions but shall not reference itself. | Parser | SS10.5, PDF p. 19 |
| ECMA55-DEF-008 | A function shall be defined at most once in a program. | Parser | SS10.5, PDF p. 19 |

## Section 11: LET Statement

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-LET-001 | `LET` shall assign the value of a numeric expression to a numeric variable or the value of a string expression to a string variable. | Runtime | SS11.1-11.4, PDF p. 19 |
| ECMA55-LET-002 | The expression on the right side of `LET` shall be evaluated before assignment. | Runtime | SS11.4, PDF p. 19 |
| ECMA55-LET-003 | Assigning a string datum that exceeds the maximum retained string length shall raise a fatal exception. | Runtime | SS11.5, PDF p. 19 |

## Section 12: Control Statements

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-CTL-001 | `GOTO` shall continue execution at the specified line number. | Runtime | SS12.1-12.4, PDF pp. 20-21 |
| ECMA55-CTL-002 | `IF ... THEN line-number` shall transfer control only when the relational expression evaluates true; otherwise execution shall continue sequentially. | Runtime | SS12.1-12.4, PDF pp. 20-21 |
| ECMA55-CTL-003 | String equality shall be true only when both strings have identical length and identical character sequence. | Runtime | SS12.4, PDF p. 21 |
| ECMA55-CTL-004 | `GOSUB` shall push the current line number and continue at the target line. | Runtime | SS12.4, PDF p. 21 |
| ECMA55-CTL-005 | `RETURN` shall pop the most recent `GOSUB` line number and continue at the following line. | Runtime | SS12.4, PDF p. 21 |
| ECMA55-CTL-006 | `ON expr GOTO ...` shall round `expr` to an integer, select the corresponding target from the list starting at index 1, and continue at that line. | Runtime | SS12.4, PDF p. 21 |
| ECMA55-CTL-007 | All line-number references in control statements shall refer to lines present in the program. | Parser | SS12.4, PDF p. 21 |
| ECMA55-CTL-008 | `STOP` shall terminate the program. | Runtime | SS12.4, PDF p. 21 |
| ECMA55-CTL-009 | Executing `RETURN` without a corresponding active `GOSUB` shall raise a fatal exception. | Runtime | SS12.5, PDF p. 21 |
| ECMA55-CTL-010 | `ON ... GOTO` shall raise a fatal exception when the rounded selector is less than 1 or greater than the number of targets listed. | Runtime | SS12.5, PDF p. 22 |

## Section 13: FOR And NEXT

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-FOR-001 | `FOR` and `NEXT` shall define loops over a simple numeric control variable. | Parser | SS13.1-13.4, PDF pp. 22-23 |
| ECMA55-FOR-002 | If the `STEP` clause is omitted, the increment shall default to `+1`. | Runtime | SS13.4, PDF p. 23 |
| ECMA55-FOR-003 | A `for-block` shall be the contiguous physical sequence from a `FOR` through the first `NEXT` using the same control variable. | Parser | SS13.4, PDF p. 22 |
| ECMA55-FOR-004 | Nested `for-block`s shall be allowed only when they are properly nested and use different control variables. Interleaving shall not be allowed. | Parser | SS13.4, PDF p. 22 |
| ECMA55-FOR-005 | Entering a `for-body` by a transfer of control other than `RETURN` shall not be allowed. | Runtime | SS13.4, PDF p. 23 |
| ECMA55-FOR-006 | Exiting a loop via `NEXT` shall leave the control variable holding the first unused value. | Runtime | SS13.6, PDF p. 23 |

## Section 14: PRINT

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-PRN-001 | `PRINT` items shall be expressions, `TAB(...)` calls, or null items separated by commas or semicolons. | Parser | SS14.1-14.3, PDF p. 24 |
| ECMA55-PRN-002 | Numeric output shall include a leading space for positive numbers or a leading minus sign for negative numbers and a trailing space after the numeric representation. | Runtime | SS14.4, PDF pp. 24-25 |
| ECMA55-PRN-003 | The implementation shall define `significance-width` `d`, with `d >= 6`, for formatted numeric output. | Documentation | SS14.4, PDF p. 24 |
| ECMA55-PRN-004 | The implementation shall define `exrad-width` `e`, with `e >= 2`, for formatted numeric output. | Documentation | SS14.4, PDF p. 24 |
| ECMA55-PRN-005 | Exact integers representable with `d` or fewer decimal digits shall print in implicit-point form. | Runtime | SS14.4, PDF p. 24 |
| ECMA55-PRN-006 | Other numbers shall print in explicit-point unscaled or scaled form according to the accuracy rules in Section 14. | Runtime | SS14.4, PDF pp. 24-25 |
| ECMA55-PRN-007 | A semicolon separator shall generate a zero-length output string. | Runtime | SS14.4, PDF p. 25 |
| ECMA55-PRN-008 | The implementation shall define the output margin, number of print zones, and print-zone length. | Documentation | SS14.4, PDF p. 25 |
| ECMA55-PRN-009 | A comma separator shall advance output to the beginning of the next print zone, or generate end-of-line when already in the last zone. | Runtime | SS14.4, PDF p. 26 |
| ECMA55-PRN-010 | A `TAB(n)` call shall move the current print position to column `n`, wrapping by the output margin when `n > margin`. | Runtime | SS14.4, PDF pp. 25-26 |
| ECMA55-PRN-011 | If a `TAB` argument evaluates below 1, a nonfatal exception shall occur and the recommended recovery shall be to use 1 and continue. | Runtime | SS14.5, PDF p. 26 |
| ECMA55-PRN-012 | If the print list does not end with a separator, `PRINT` shall generate an end-of-line. | Runtime | SS14.4, PDF p. 26 |
| ECMA55-PRN-013 | If printing a nonempty item would exceed the margin, an end-of-line shall be inserted before that item. | Runtime | SS14.4, PDF p. 26 |
| ECMA55-PRN-014 | If a single printed item exceeds the margin, end-of-line markers shall be inserted every `margin` characters within that item. | Runtime | SS14.4, PDF p. 26 |

## Section 15: INPUT

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-INP-001 | `INPUT` shall assign values to variables from an input reply in the order of the variable list. | Runtime | SS15.1-15.4, PDF pp. 26-27 |
| ECMA55-INP-002 | Interactive mode shall request input by outputting an input prompt. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-003 | Batch-mode input retrieval shall be implementation-defined. | Documentation | SS15.4, PDF p. 27 |
| ECMA55-INP-004 | Program execution shall be suspended until a valid input reply has been supplied. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-005 | Numeric variables shall accept numeric constants, and string variables shall accept quoted or unquoted strings. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-006 | Leading and trailing spaces in unquoted string input for a string variable shall be ignored. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-007 | Numeric underflow while reading input shall replace the value by zero. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-008 | Subscript expressions in the variable list shall be evaluated after values have been assigned to variables to their left. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-009 | No assignments from an input reply shall occur until the reply has been validated for datum types, datum count, and allowable ranges. | Runtime | SS15.4, PDF p. 27 |
| ECMA55-INP-010 | Type mismatch, too little input, too much input, numeric overflow, and overlong string input shall raise nonfatal exceptions whose recommended recovery is to request a replacement input reply. | Runtime | SS15.5, PDF pp. 27-28 |

## Section 16: READ And RESTORE

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-RDR-001 | `READ` shall assign values from the program's data sequence to variables in left-to-right order. | Runtime | SS16.1-16.4, PDF pp. 28-29 |
| ECMA55-RDR-002 | A conceptual data pointer shall start at the first datum when program execution begins. | Runtime | SS16.4, PDF p. 28 |
| ECMA55-RDR-003 | Each `READ` shall consume data sequentially and advance the data pointer past each assigned datum. | Runtime | SS16.4, PDF p. 28 |
| ECMA55-RDR-004 | `RESTORE` shall reset the data pointer to the beginning of the data sequence. | Runtime | SS16.4, PDF p. 28 |
| ECMA55-RDR-005 | Data types shall match the target variable types, except that an unquoted string that is a valid numeric representation may be assigned to either string or numeric variables. | Runtime | SS16.4, PDF p. 29 |
| ECMA55-RDR-006 | Numeric underflow while reading data shall replace the value by zero. | Runtime | SS16.4, PDF p. 29 |
| ECMA55-RDR-007 | Subscript expressions in the variable list shall be evaluated after assignments to variables to their left. | Runtime | SS16.4, PDF p. 29 |
| ECMA55-RDR-008 | Reading past the remaining data sequence shall raise a fatal exception. | Runtime | SS16.5, PDF p. 29 |
| ECMA55-RDR-009 | Assigning a string datum to a numeric variable via `READ` shall raise a fatal exception. | Runtime | SS16.5, PDF p. 29 |
| ECMA55-RDR-010 | Numeric overflow during `READ` shall raise a nonfatal exception, with recommended recovery by supplying machine infinity with the appropriate sign and continuing. | Runtime | SS16.5, PDF p. 29 |
| ECMA55-RDR-011 | Reading a string datum that exceeds the retained string length shall raise a fatal exception. | Runtime | SS16.5, PDF p. 29 |

## Section 17: DATA

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-DAT-001 | `DATA` shall construct the data sequence used by `READ`. | Runtime | SS17.1-17.4, PDF pp. 29-30 |
| ECMA55-DAT-002 | The global data sequence shall preserve the textual order of data across all `DATA` statements in the program. | Runtime | SS17.4, PDF p. 29 |
| ECMA55-DAT-003 | Executing a `DATA` line directly shall have no effect other than continuing to the next line. | Runtime | SS17.4, PDF p. 30 |

## Section 18: Array Declarations

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-ARR-001 | `DIM` shall declare one-dimensional or two-dimensional arrays and their upper bounds. | Parser | SS18.1-18.4, PDF pp. 30-31 |
| ECMA55-ARR-002 | In the absence of explicit declarations, array lower bounds shall default to `0` and upper bounds shall default to `10`. | Runtime | SS18.1, PDF p. 30 |
| ECMA55-ARR-003 | `OPTION BASE 0` or `OPTION BASE 1` shall declare the lower bound for all arrays. | Parser | SS18.1-18.4, PDF pp. 30-31 |
| ECMA55-ARR-004 | A `DIM` declaration, if present, shall appear on a lower-numbered line than any reference to the declared array. | Parser | SS18.4, PDF p. 30 |
| ECMA55-ARR-005 | An `OPTION BASE` statement, if present, shall appear on a lower-numbered line than any `DIM` statement or array-element reference. | Parser | SS18.4, PDF p. 31 |
| ECMA55-ARR-006 | If `OPTION BASE 1` is used, no `DIM` bound may be zero. | Parser | SS18.4, PDF p. 31 |
| ECMA55-ARR-007 | A program shall contain at most one `OPTION BASE` statement. | Parser | SS18.4, PDF p. 31 |
| ECMA55-ARR-008 | An array shall be explicitly dimensioned at most once. | Parser | SS18.4, PDF p. 31 |
| ECMA55-ARR-009 | Executing `DIM` or `OPTION BASE` directly shall have no effect other than continuing to the next line. | Runtime | SS18.4, PDF p. 31 |

## Section 19: Remark

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-REM-001 | `REM` shall allow program annotation using a remark string. | Parser | SS19.1-19.4, PDF p. 31 |
| ECMA55-REM-002 | Executing a `REM` line shall have no effect other than continuing to the next line. | Runtime | SS19.4, PDF p. 31 |

## Section 20: RANDOMIZE

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-RND-001 | `RANDOMIZE` shall generate a new unpredictable starting point for the pseudo-random sequence used by `RND`. | Runtime | SS20.1-20.4, PDF pp. 31-32 |
| ECMA55-RND-002 | If no randomizing device is available, `RANDOMIZE` may be implemented through interaction with the user. | Manual | SS20.6, PDF p. 32 |

## Appendix 3: Conformance

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-CNF-001 | A conforming program shall contain only statements that are syntactically valid instances of statements defined by the standard. | Parser | Appendix 3, PDF p. 39 |
| ECMA55-CNF-002 | A conforming program shall assign only meanings explicitly defined by the standard to its statements and overall composition. | Manual | Appendix 3, PDF p. 39 |
| ECMA55-CNF-003 | A conforming implementation shall accept and process standard-conforming programs. | Runtime | Appendix 3, PDF p. 39 |
| ECMA55-CNF-004 | A conforming implementation shall report reasons for rejecting a non-conforming program. | Runtime | Appendix 3, PDF p. 39 |
| ECMA55-CNF-005 | A conforming implementation shall interpret semantics, errors, and exceptions according to the standard. | Runtime | Appendix 3, PDF p. 39 |
| ECMA55-CNF-006 | A conforming implementation shall accept, manipulate, and generate numeric values with at least the precision and range required by the standard. | Runtime | Appendix 3, PDF p. 39 |
| ECMA55-CNF-007 | A conforming implementation shall be accompanied by a reference manual that defines all undefined and implementation-defined behavior. | Documentation | Appendix 3, PDF p. 39 |
| ECMA55-CNF-008 | When a program element does not conform syntactically, the implementation shall either report an error or assign it an implementation-defined meaning. | Runtime | Appendix 3, PDF p. 39 |

## Appendix 4: Implementation-Defined Features

| ID | Requirement | Verification | Source |
| --- | --- | --- | --- |
| ECMA55-DOC-001 | The reference manual shall define numeric-expression evaluation accuracy. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-002 | The reference manual shall define end-of-line handling. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-003 | The reference manual shall define print numeric `exrad-width`. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-004 | The reference manual shall define initial variable values. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-005 | The reference manual shall define the input prompt. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-006 | The reference manual shall define the maximum retained string length. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-007 | The reference manual shall define machine infinitesimal. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-008 | The reference manual shall define machine infinity. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-009 | The reference manual shall define the output margin. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-010 | The reference manual shall define numeric precision. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-011 | The reference manual shall define print-zone length. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-012 | The reference manual shall define the pseudo-random number sequence used by `RND` when `RANDOMIZE` is not applied. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-013 | The reference manual shall define print numeric `significance-width`. | Documentation | Appendix 4, PDF p. 40 |
| ECMA55-DOC-014 | The reference manual shall define how batch-mode input replies are requested. | Documentation | Appendix 4, PDF p. 40 |
