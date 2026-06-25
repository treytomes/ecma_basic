using System;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for DEF FN user-defined functions per ECMA-55 Section 10.
/// ECMA55-DEF-001 to DEF-008: User-defined numeric functions.
/// </summary>
public class DefFnTests
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

	#region Phase 1: Zero-Parameter Functions (ECMA55-DEF-001)

	[Fact]
	public void DefFn_ZeroParameters_DefinesConstant()
	{
		// DEF FNA = 3.14159
		var program = @"10 DEF FNA = 3.14159
20 PRINT FNA
30 END
";

		var result = RunProgram(program);
		Assert.Contains("3.14159", result);
	}

	[Fact]
	public void DefFn_ZeroParameters_UsesGlobalVariables()
	{
		// ECMA55-DEF-004: Non-parameter variables refer to global scope
		var program = @"10 LET X = 5
20 DEF FNA = X * 2
30 PRINT FNA
40 END
";

		var result = RunProgram(program);
		Assert.Contains("10", result);
	}

	[Fact]
	public void DefFn_ZeroParameters_MultipleDefinitions()
	{
		var program = @"10 DEF FNA = 1
20 DEF FNB = 2
30 DEF FNC = 3
40 PRINT FNA
50 PRINT FNB
60 PRINT FNC
70 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("1", lines[0]);
		Assert.Contains("2", lines[1]);
		Assert.Contains("3", lines[2]);
	}

	[Fact]
	public void DefFn_ExecutingDefLine_ContinuesToNextLine()
	{
		// ECMA55-DEF-006: Executing DEF line has no effect (just continues)
		var program = @"10 DEF FNA = 42
20 PRINT ""AFTER DEF""
30 PRINT FNA
40 END
";

		var result = RunProgram(program);
		Assert.Contains("AFTER DEF", result);
		Assert.Contains("42", result);
	}

	[Fact]
	public void DefFn_CanUseInExpression()
	{
		var program = @"10 DEF FNA = 10
20 PRINT FNA + 5
30 PRINT FNA * 2
40 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("15", lines[0]);
		Assert.Contains("20", lines[1]);
	}

	#endregion

	#region Phase 3: Validation - Use Before Definition (ECMA55-DEF-005)

	[Fact]
	public void DefFn_UseBeforeDefinition_ReportsError()
	{
		// ECMA55-DEF-005: Definition must appear before first reference
		var program = @"10 PRINT FNA
20 DEF FNA = 42
30 END
";

		var result = RunProgram(program);
		Assert.Contains("UNDEFINED FUNCTION FNA", result.ToUpper());
	}

	#endregion

	#region Phase 3: Validation - Duplicate Definition (ECMA55-DEF-008)

	[Fact]
	public void DefFn_DuplicateDefinition_ReportsError()
	{
		// ECMA55-DEF-008: Function defined at most once per program
		var program = @"10 DEF FNA = 1
20 DEF FNA = 2
30 END
";

		var result = RunProgram(program);
		Assert.Contains("ALREADY DEFINED", result.ToUpper());
	}

	#endregion

	#region Phase 3: Name Validation

	[Fact]
	public void DefFn_NameMustBeSingleLetter()
	{
		// Function name must be FN followed by single letter
		var program = @"10 DEF FNAB = 42
20 END
";

		// Should fail during parsing
		var result = RunProgram(program);
		Assert.Contains("SINGLE LETTER", result.ToUpper());
	}

	[Fact]
	public void DefFn_ZeroParameterCannotBeCalledWithArgument()
	{
		// Zero-parameter function should reject arguments
		var program = @"10 DEF FNA = 42
20 PRINT FNA(5)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("TAKES NO PARAMETERS", result.ToUpper());
	}

	[Fact]
	public void DefFn_OneParameterRequiresArgument()
	{
		// One-parameter function requires an argument
		var program = @"10 DEF FNB(X) = X * 2
20 PRINT FNB
30 END
";

		var result = RunProgram(program);
		Assert.Contains("REQUIRES ONE PARAMETER", result.ToUpper());
	}

	[Fact]
	public void DefFn_RecursiveCall_ReportsError()
	{
		// ECMA55-DEF-007: Function may call other functions but not itself
		var program = @"10 DEF FNA(X) = FNA(X - 1) + 1
20 PRINT FNA(5)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("RECURSIVE", result.ToUpper());
	}

	[Fact]
	public void DefFn_FunctionCallingOtherFunction_Works()
	{
		// ECMA55-DEF-007: Function MAY call OTHER functions (just not itself)
		var program = @"10 DEF FNA(X) = X * 2
20 DEF FNB(Y) = FNA(Y) + 5
30 PRINT FNB(3)
40 END
";

		var result = RunProgram(program);
		Assert.Contains("11", result); // FNB(3) = FNA(3) + 5 = 6 + 5 = 11
	}

	#endregion

	#region Phase 2: One-Parameter Functions (ECMA55-DEF-001, DEF-002)

	[Fact]
	public void DefFn_OneParameter_BasicEvaluation()
	{
		// DEF FNB(X) = X * X
		var program = @"10 DEF FNB(X) = X * X
20 PRINT FNB(5)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("25", result);
	}

	[Fact]
	public void DefFn_OneParameter_ParameterShadowsGlobal()
	{
		// ECMA55-DEF-002: Parameter shadows global variable with same name
		var program = @"10 LET X = 100
20 DEF FNB(X) = X * 2
30 PRINT FNB(5)
40 PRINT X
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("10", lines[0]);  // FNB(5) = 5 * 2 = 10
		Assert.Contains("100", lines[1]); // Global X unchanged
	}

	[Fact]
	public void DefFn_OneParameter_AccessesGlobalVariables()
	{
		// ECMA55-DEF-004: Non-parameter variables refer to global scope
		var program = @"10 LET Y = 10
20 DEF FNC(X) = X + Y
30 PRINT FNC(5)
40 END
";

		var result = RunProgram(program);
		Assert.Contains("15", result); // 5 + 10 = 15
	}

	[Fact]
	public void DefFn_OneParameter_ComplexExpression()
	{
		var program = @"10 DEF FND(X) = X * X + 2 * X + 1
20 PRINT FND(3)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("16", result); // 3*3 + 2*3 + 1 = 9 + 6 + 1 = 16
	}

	[Fact]
	public void DefFn_OneParameter_MultipleCallsWithDifferentArguments()
	{
		var program = @"10 DEF FNE(X) = X * 3
20 PRINT FNE(2)
30 PRINT FNE(4)
40 PRINT FNE(10)
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("6", lines[0]);   // 2 * 3 = 6
		Assert.Contains("12", lines[1]);  // 4 * 3 = 12
		Assert.Contains("30", lines[2]);  // 10 * 3 = 30
	}

	#endregion

	#region Edge Cases and Integration Tests

	[Fact]
	public void DefFn_NegativeNumbers()
	{
		var program = @"10 DEF FNA(X) = X * -1
20 PRINT FNA(5)
30 PRINT FNA(-3)
40 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("-5", lines[0]);
		Assert.Contains("3", lines[1]);
	}

	[Fact]
	public void DefFn_DivisionAndFractions()
	{
		var program = @"10 DEF FNA(X) = X / 2
20 PRINT FNA(10)
30 PRINT FNA(7)
40 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("5", lines[0]);
		Assert.Contains("3.5", lines[1]);
	}

	[Fact]
	public void DefFn_InIfStatement()
	{
		var program = @"10 DEF FNA(X) = X * 2
20 IF FNA(3) > 5 THEN 50
30 PRINT ""SMALL""
40 GOTO 60
50 PRINT ""LARGE""
60 END
";

		var result = RunProgram(program);
		Assert.Contains("LARGE", result);
		Assert.DoesNotContain("SMALL", result);
	}

	[Fact]
	public void DefFn_InForLoop()
	{
		var program = @"10 DEF FNA(X) = X * X
20 FOR I = 1 TO 3
30 PRINT FNA(I)
40 NEXT I
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("1", lines[0]);  // 1*1
		Assert.Contains("4", lines[1]);  // 2*2
		Assert.Contains("9", lines[2]);  // 3*3
	}

	[Fact]
	public void DefFn_WithIntrinsicFunction()
	{
		var program = @"10 DEF FNA(X) = SQR(X) + 1
20 PRINT FNA(16)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("5", result); // SQR(16) + 1 = 4 + 1 = 5
	}

	[Fact]
	public void DefFn_ZeroParameterInForLoop()
	{
		var program = @"10 DEF FNA = 2
20 FOR I = 1 TO FNA
30 PRINT I
40 NEXT I
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("1", lines[0]);
		Assert.Contains("2", lines[1]);
		Assert.Equal(2, lines.Length);
	}

	[Fact]
	public void DefFn_NestedFunctionCalls_ThreeDeep()
	{
		var program = @"10 DEF FNA(X) = X + 1
20 DEF FNB(Y) = FNA(Y) * 2
30 DEF FNC(Z) = FNB(Z) + 10
40 PRINT FNC(5)
50 END
";

		var result = RunProgram(program);
		Assert.Contains("22", result); // FNC(5) = FNB(5)+10 = (FNA(5)*2)+10 = (6*2)+10 = 22
	}

	[Fact]
	public void DefFn_ParameterShadowsMultipleGlobals()
	{
		// Parameter shadows global, but non-parameter globals still accessible
		var program = @"10 LET A = 10
20 LET B = 5
30 DEF FNA(A) = A + B
40 PRINT FNA(3)
50 PRINT A
60 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("8", lines[0]);   // FNA(3) = 3 + B = 3 + 5 = 8
		Assert.Contains("10", lines[1]);  // Global A unchanged
	}

	[Fact]
	public void DefFn_MultipleFunctionsWithSameParameterName()
	{
		// Each function has independent parameter scope
		var program = @"10 DEF FNA(X) = X * 2
20 DEF FNB(X) = X + 10
30 PRINT FNA(5)
40 PRINT FNB(5)
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("10", lines[0]);  // FNA(5) = 5 * 2 = 10
		Assert.Contains("15", lines[1]);  // FNB(5) = 5 + 10 = 15
	}

	[Fact]
	public void DefFn_MultipleCallsInOneExpression()
	{
		var program = @"10 DEF FNA(X) = X * 2
20 PRINT FNA(3) + FNA(4) + FNA(5)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("24", result); // 6 + 8 + 10 = 24
	}

	[Fact]
	public void DefFn_AllSingleLetterNames()
	{
		// Test that all letters A-Z work as function names
		var program = @"10 DEF FNA = 1
20 DEF FNZ = 26
30 PRINT FNA
40 PRINT FNZ
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("1", lines[0]);
		Assert.Contains("26", lines[1]);
	}

	[Fact]
	public void DefFn_ExpressionWithOperatorPrecedence()
	{
		var program = @"10 DEF FNA(X) = X * 2 + 3
20 DEF FNB(Y) = (Y + 3) * 2
30 PRINT FNA(5)
40 PRINT FNB(5)
50 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("13", lines[0]);  // 5 * 2 + 3 = 10 + 3 = 13
		Assert.Contains("16", lines[1]);  // (5 + 3) * 2 = 8 * 2 = 16
	}

	[Fact]
	public void DefFn_WithInvolution()
	{
		var program = @"10 DEF FNA(X) = X ^ 3
20 PRINT FNA(2)
30 PRINT FNA(3)
40 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("8", lines[0]);   // 2^3 = 8
		Assert.Contains("27", lines[1]);  // 3^3 = 27
	}

	[Fact]
	public void DefFn_GlobalVariableChangesAffectFunction()
	{
		// ECMA55-DEF-004: Non-parameter variables refer to global scope
		var program = @"10 LET Y = 10
20 DEF FNA = Y * 2
30 PRINT FNA
40 LET Y = 5
50 PRINT FNA
60 END
";

		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		Assert.Contains("20", lines[0]);  // Y=10, FNA = 20
		Assert.Contains("10", lines[1]);  // Y=5, FNA = 10
	}

	[Fact]
	public void DefFn_FunctionReferencedBeforeDefButNotCalled()
	{
		// Defining FNA after FNB is OK if FNB doesn't call FNA until after FNA is defined
		var program = @"10 DEF FNB(X) = X + 5
20 DEF FNA(Y) = FNB(Y) * 2
30 PRINT FNA(3)
40 END
";

		var result = RunProgram(program);
		Assert.Contains("16", result); // FNA(3) = FNB(3) * 2 = 8 * 2 = 16
	}

	[Fact]
	public void DefFn_WithBooleanComparison()
	{
		// Functions can be used in boolean comparisons
		var program = @"10 DEF FNA(X) = X * 2
20 IF FNA(5) = 10 THEN 40
30 PRINT ""NO""
40 PRINT ""YES""
50 END
";

		var result = RunProgram(program);
		Assert.Contains("YES", result);
	}

	[Fact]
	public void DefFn_SpaceAfterFN_ParsesCorrectly()
	{
		// Regression test: Space between FN and letter should be allowed
		// Bug: Parser wasn't consuming optional space after "FN" token
		var program = @"10 LET P = 3.14159
20 DEF FN D(X) = P * X / 180
30 PRINT FND(90)
40 END
";

		var result = RunProgram(program);
		Assert.Contains("1.57079", result); // 90 * PI / 180 ≈ 1.57079
	}

	#endregion
}
