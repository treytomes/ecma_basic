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

	#region Phase 1: Validation - Use Before Definition (ECMA55-DEF-005)

	[Fact]
	public void DefFn_UseBeforeDefinition_ThrowsError()
	{
		// ECMA55-DEF-005: Definition must appear before first reference
		var program = @"10 PRINT FNA
20 DEF FNA = 42
30 END
";

		var exception = Assert.ThrowsAny<Exception>(() => RunProgram(program));
		Assert.Contains("UNDEFINED", exception.Message.ToUpper());
	}

	#endregion

	#region Phase 1: Validation - Duplicate Definition (ECMA55-DEF-008)

	[Fact]
	public void DefFn_DuplicateDefinition_ThrowsError()
	{
		// ECMA55-DEF-008: Function defined at most once per program
		var program = @"10 DEF FNA = 1
20 DEF FNA = 2
30 END
";

		var exception = Assert.ThrowsAny<Exception>(() => RunProgram(program));
		Assert.Contains("ALREADY DEFINED", exception.Message.ToUpper());
	}

	#endregion

	#region Phase 1: Name Validation

	[Fact]
	public void DefFn_NameMustBeSingleLetter()
	{
		// Function name must be FN followed by single letter
		var program = @"10 DEF FNAB = 42
20 END
";

		// Should fail during parsing
		var exception = Assert.ThrowsAny<Exception>(() => RunProgram(program));
		Assert.NotNull(exception);
	}

	#endregion
}
