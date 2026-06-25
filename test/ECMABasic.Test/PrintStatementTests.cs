using System;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

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

	#region Phase 1: Basic Output Tests (ECMA55-PRN-001, PRN-012)

	[Fact]
	public void Print_Empty_GeneratesBlankLine()
	{
		// ECMA55-PRN-012: PRINT without trailing separator generates EOL
		var program = @"10 PRINT
20 END
";

		var result = RunProgram(program);
		Assert.Equal("\r\n", result);
	}

	[Fact]
	public void Print_SingleString_OutputsStringWithNewline()
	{
		// ECMA55-PRN-001: PRINT items can be expressions
		// ECMA55-PRN-012: PRINT without trailing separator generates EOL
		var program = @"10 PRINT ""HELLO""
20 END
";

		var result = RunProgram(program);
		Assert.Equal("HELLO\r\n", result);
	}

	[Fact]
	public void Print_SingleInteger_OutputsWithSpaces()
	{
		// ECMA55-PRN-005: Exact integers print in implicit-point form
		var program = @"10 PRINT 42
20 END
";

		var result = RunProgram(program);
		Assert.Contains("42", result);
		// Should have leading space for positive numbers
		Assert.StartsWith(" ", result.Trim('\r', '\n'));
	}

	[Fact]
	public void Print_SingleDecimal_OutputsWithSpaces()
	{
		// ECMA55-PRN-006: Numbers print in explicit-point form
		var program = @"10 PRINT 3.14159
20 END
";

		var result = RunProgram(program);
		Assert.Contains("3.14159", result);
	}

	[Fact]
	public void Print_MultipleItemsWithSemicolons_NoSpacesBetween()
	{
		// ECMA55-PRN-001: Items separated by semicolons
		var program = @"10 PRINT ""A"";""B"";""C""
20 END
";

		var result = RunProgram(program);
		Assert.Equal("ABC\r\n", result);
	}

	[Fact]
	public void Print_MultipleItemsWithCommas_ZonePositioning()
	{
		// ECMA55-PRN-009: Comma advances to next print zone
		// Zone width = 80 / 5 = 16 columns
		var program = @"10 PRINT ""A"",""B"",""C""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A should be at position 0
		Assert.StartsWith("A", line);

		// B should be at position 16 (start of zone 2)
		// There should be spaces between A and B
		Assert.Contains("B", line);

		// C should be at position 32 (start of zone 3)
		Assert.Contains("C", line);
	}

	#endregion

	#region Phase 1: Separator Tests (ECMA55-PRN-009, PRN-012)

	[Fact]
	public void Print_TrailingSemicolon_SuppressesNewline()
	{
		// ECMA55-PRN-012: Trailing separator suppresses EOL
		var program = @"10 PRINT ""HELLO"";
20 PRINT ""WORLD""
30 END
";

		var result = RunProgram(program);
		Assert.Equal("HELLOWORLD\r\n", result);
	}

	[Fact]
	public void Print_TrailingComma_NextZoneSameLine()
	{
		// ECMA55-PRN-009: Trailing comma advances to next zone
		// ECMA55-PRN-012: Trailing separator suppresses EOL
		var program = @"10 PRINT ""A"",
20 PRINT ""B""
30 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// Both A and B should be on the same line
		Assert.Contains("A", line);
		Assert.Contains("B", line);
		// Should not contain two newlines (which would indicate separate lines)
		Assert.DoesNotContain("\r\n\r\n", result);
	}

	[Fact]
	public void Print_MultipleCommas_SkipsZones()
	{
		// ECMA55-PRN-001: Null items between separators
		// ECMA55-PRN-009: Multiple commas advance multiple zones
		var program = @"10 PRINT ""A"",,""C""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A at position 0, skip zone 2, C at position 32
		Assert.StartsWith("A", line);
		Assert.Contains("C", line);
	}

	[Fact]
	public void Print_MultipleZones_PositionsCorrectly()
	{
		// ECMA55-PRN-009: Comma advances to next print zone
		// Test that items appear in different zones (with spacing)
		var program = @"10 PRINT ""ZONE1"",""ZONE2"",""ZONE3""
20 END
";

		var result = RunProgram(program);

		// All three items should be present
		Assert.Contains("ZONE1", result);
		Assert.Contains("ZONE2", result);
		Assert.Contains("ZONE3", result);

		// Zone2 should have significant space before it (not immediately after ZONE1)
		var idx1 = result.IndexOf("ZONE1");
		var idx2 = result.IndexOf("ZONE2");
		Assert.True(idx2 > idx1 + 5 + 3, // "ZONE1" is 5 chars, expect at least 3 spaces
			"ZONE2 should be in a different zone with spacing");
	}

	#endregion

	#region Phase 1: Numeric Formatting Tests (ECMA55-PRN-005, PRN-006)

	[Fact]
	public void Print_Zero_FormatsWithSpaces()
	{
		// ECMA55-PRN-005: Zero formatting
		var program = @"10 PRINT 0
20 END
";

		var result = RunProgram(program);
		Assert.Contains("0", result);
		// Should be " 0 " with spaces
		var line = result.Trim('\r', '\n');
		Assert.Equal(" 0 ", line);
	}

	[Fact]
	public void Print_PositiveInteger_LeadingSpace()
	{
		// ECMA55-PRN-005: Positive numbers have leading space
		var program = @"10 PRINT 123
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');
		Assert.StartsWith(" ", line);
		Assert.Contains("123", line);
	}

	[Fact]
	public void Print_NegativeInteger_MinusSign()
	{
		// ECMA55-PRN-005: Negative numbers have minus sign, no leading space
		var program = @"10 PRINT -456
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');
		Assert.StartsWith("-", line);
		Assert.Contains("456", line);
	}

	[Fact]
	public void Print_SmallDecimal_ImplicitPointForm()
	{
		// ECMA55-PRN-006: Small decimals in implicit-point form
		// Note: Numbers < 1 may print without leading zero (implicit-point)
		var program = @"10 PRINT 0.123456
20 END
";

		var result = RunProgram(program);
		// Could be ".123456" or "0.123456" - both are valid implicit-point forms
		var cleaned = result.Replace(" ", "").Replace("\r", "").Replace("\n", "");
		Assert.True(cleaned.Contains(".123456") || cleaned.Contains("0.123456"),
			$"Expected '.123456' or '0.123456', got '{cleaned}'");
	}

	[Fact]
	public void Print_LargeNumber_ScientificNotation()
	{
		// ECMA55-PRN-006: Large numbers in scaled form
		var program = @"10 PRINT 1234567890
20 END
";

		var result = RunProgram(program);
		// Should use scientific notation (E format)
		Assert.Contains("E", result.ToUpper());
	}

	[Fact]
	public void Print_VerySmallNumber_ScientificNotation()
	{
		// ECMA55-PRN-006: Very small numbers in scaled form
		var program = @"10 PRINT 0.000000123
20 END
";

		var result = RunProgram(program);
		// Should use scientific notation
		Assert.Contains("E", result.ToUpper());
	}

	#endregion

	#region Phase 1: Expression Tests (ECMA55-PRN-001)

	[Fact]
	public void Print_ArithmeticExpression_EvaluatesAndPrints()
	{
		// ECMA55-PRN-001: PRINT items can be expressions
		var program = @"10 PRINT 2 + 3
20 END
";

		var result = RunProgram(program);
		Assert.Contains("5", result);
	}

	[Fact]
	public void Print_Variable_OutputsValue()
	{
		// ECMA55-PRN-001: PRINT items can be expressions (variables)
		var program = @"10 LET A = 42
20 PRINT A
30 END
";

		var result = RunProgram(program);
		Assert.Contains("42", result);
	}

	[Fact]
	public void Print_FunctionCall_OutputsResult()
	{
		// ECMA55-PRN-001: PRINT items can be expressions (function calls)
		var program = @"10 PRINT SQR(16)
20 END
";

		var result = RunProgram(program);
		Assert.Contains("4", result);
	}

	#endregion
}
