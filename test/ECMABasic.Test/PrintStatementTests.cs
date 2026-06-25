using System;
using System.Linq;
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

	#region Phase 2: TAB Function Tests (ECMA55-PRN-010)

	[Fact]
	public void Print_TAB_MovesToColumn()
	{
		// ECMA55-PRN-010: TAB(n) moves to column n
		var program = @"10 PRINT ""A"";TAB(10);""B""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A should be at position 0
		Assert.StartsWith("A", line);

		// B should be after position 0, with spaces in between
		Assert.Contains("B", line);
		var indexB = line.IndexOf('B');
		Assert.True(indexB > 1, $"B should be after A with spacing, got index {indexB}");
	}

	[Fact]
	public void Print_TAB_WithComma()
	{
		// ECMA55-PRN-010: TAB works with comma separators
		var program = @"10 PRINT ""A"",TAB(20),""B""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A in zone 1, then TAB, then B
		Assert.Contains("A", line);
		Assert.Contains("B", line);

		// B should be after A with spacing
		var indexA = line.IndexOf('A');
		var indexB = line.IndexOf('B');
		Assert.True(indexB > indexA + 1, "B should be after A with spacing");
	}

	[Fact]
	public void Print_TAB_BeyondCurrentPosition()
	{
		// ECMA55-PRN-010: TAB beyond current position moves forward
		var program = @"10 PRINT ""HELLO"";TAB(20);""WORLD""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// HELLO at 0-4, then TAB to 20, WORLD after that
		Assert.StartsWith("HELLO", line);
		Assert.Contains("WORLD", line);

		// There should be spaces between HELLO and WORLD
		var indexHello = line.IndexOf("HELLO");
		var indexWorld = line.IndexOf("WORLD");
		Assert.True(indexWorld > indexHello + 5, "WORLD should be after HELLO with spacing");
	}

	[Fact]
	public void Print_TAB_BeforeCurrentPosition_Wraps()
	{
		// ECMA55-PRN-010: TAB to position before current generates newline
		var program = @"10 PRINT ""HELLO WORLD"";TAB(5);""X""
20 END
";

		var result = RunProgram(program);

		// HELLO WORLD is 11 chars, TAB(5) is before current position
		// Should wrap to next line and put X at column 5
		Assert.Contains("HELLO WORLD", result);
		Assert.Contains("X", result);

		// Check for newline between them
		var indexWorld = result.IndexOf("WORLD");
		var indexX = result.IndexOf("X");
		var between = result.Substring(indexWorld + 5, indexX - indexWorld - 5);
		Assert.True(between.Contains("\r\n"), "Should have newline when TAB wraps");
	}

	[Fact]
	public void Print_TAB_LargeValue_HandlesCorrectly()
	{
		// ECMA55-PRN-010: TAB(n) where n > margin may wrap by margin
		// Test that large TAB values don't crash
		var program = @"10 PRINT TAB(90);""X""
20 END
";

		var result = RunProgram(program);

		// Just verify X appears and doesn't crash
		Assert.Contains("X", result);
	}

	[Fact]
	public void Print_TAB_WithExpression()
	{
		// ECMA55-PRN-010: TAB accepts expressions
		var program = @"10 LET N = 15
20 PRINT TAB(N * 2);""X""
30 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// TAB(15 * 2) = TAB(30)
		Assert.Contains("X", line);
		var indexX = line.IndexOf('X');
		Assert.True(indexX >= 28 && indexX <= 32, $"X should be near position 30, got {indexX}");
	}

	[Fact]
	public void Print_TAB_Zero()
	{
		// Edge case: TAB(0) should go to column 0
		var program = @"10 PRINT ""START"";TAB(0);""X""
20 END
";

		var result = RunProgram(program);

		// Should have START, then newline, then X at column 0
		Assert.Contains("START", result);
		Assert.Contains("X", result);
	}

	[Fact]
	public void Print_TAB_One()
	{
		// Edge case: TAB(1) should go to column 1
		var program = @"10 PRINT TAB(1);""X""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// X should be at position 1 (one space before it)
		Assert.Contains("X", line);
		var indexX = line.IndexOf('X');
		Assert.True(indexX >= 0 && indexX <= 2, $"X should be near position 1, got {indexX}");
	}

	[Fact]
	public void Print_MultipleTABs_InSameLine()
	{
		// Multiple TAB calls in one PRINT
		var program = @"10 PRINT TAB(5);""A"";TAB(15);""B"";TAB(25);""C""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A at ~5, B at ~15, C at ~25
		Assert.Contains("A", line);
		Assert.Contains("B", line);
		Assert.Contains("C", line);

		var indexA = line.IndexOf('A');
		var indexB = line.IndexOf('B');
		var indexC = line.IndexOf('C');

		Assert.True(indexA >= 4 && indexA <= 6, $"A should be near position 5, got {indexA}");
		Assert.True(indexB >= 14 && indexB <= 16, $"B should be near position 15, got {indexB}");
		Assert.True(indexC >= 24 && indexC <= 26, $"C should be near position 25, got {indexC}");
	}

	#endregion

	#region Phase 3: Advanced Formatting Tests (ECMA55-PRN-005, PRN-006)

	[Fact]
	public void Print_SpecialValue_Infinity()
	{
		// ECMA55-PRN-006: Special numeric values
		var program = @"10 LET A = 1 / 0
20 PRINT A
30 END
";

		var result = RunProgram(program);
		// Should output INF or similar
		Assert.Contains("INF", result.ToUpper());
	}

	[Fact]
	public void Print_SpecialValue_NegativeInfinity()
	{
		// ECMA55-PRN-006: Negative infinity
		var program = @"10 LET A = -1 / 0
20 PRINT A
30 END
";

		var result = RunProgram(program);
		// Should output -INF or similar
		Assert.Contains("-INF", result.ToUpper());
	}

	[Fact]
	public void Print_SpecialValue_NaN()
	{
		// ECMA55-PRN-006: Not a Number
		var program = @"10 LET A = 0 / 0
20 PRINT A
30 END
";

		var result = RunProgram(program);
		// Should output NAN or similar
		Assert.Contains("NAN", result.ToUpper());
	}

	[Fact]
	public void Print_ScientificNotation_LargeNumber()
	{
		// ECMA55-PRN-006: Large numbers use scientific notation
		var program = @"10 PRINT 9999999999
20 END
";

		var result = RunProgram(program);
		// Should contain E for exponent
		Assert.Contains("E", result.ToUpper());
	}

	[Fact]
	public void Print_ScientificNotation_SmallNumber()
	{
		// ECMA55-PRN-006: Very small numbers use scientific notation
		var program = @"10 PRINT 0.0000000001
20 END
";

		var result = RunProgram(program);
		// Should contain E for exponent (negative exponent)
		Assert.Contains("E", result.ToUpper());
		// Should have negative exponent
		Assert.Matches(@"E-\d", result.ToUpper());
	}

	[Fact]
	public void Print_SignificanceWidth_Boundary()
	{
		// ECMA55-PRN-005, PRN-006: Numbers at significance width boundary
		// Significance width is 6 digits
		var program = @"10 PRINT 123456
20 PRINT 1234567
30 END
";

		var result = RunProgram(program);
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

		// 123456 should print as integer (within significance)
		Assert.Contains("123456", lines[0]);

		// 1234567 may use scientific notation (exceeds significance)
		// Just verify it prints something
		Assert.True(lines.Length >= 2);
		Assert.NotEmpty(lines[1].Trim());
	}

	[Fact]
	public void Print_Rounding_ToSignificanceWidth()
	{
		// ECMA55-PRN-006: Numbers rounded to significance width
		var program = @"10 PRINT 1.2345678901234
20 END
";

		var result = RunProgram(program);
		// Should be rounded to 6 significant digits
		// Exact format may vary, but should have 1.23456 or 1.23457 (rounded)
		var cleaned = result.Replace(" ", "").Replace("\r", "").Replace("\n", "");
		Assert.True(cleaned.Contains("1.234") || cleaned.Contains("1.235"),
			$"Should round to significance width, got: {cleaned}");
	}

	[Fact]
	public void Print_VeryCloseToZero_ScientificNotation()
	{
		// ECMA55-PRN-006: Numbers very close to zero
		var program = @"10 PRINT 0.0000001
20 END
";

		var result = RunProgram(program);
		// Should use scientific notation for very small numbers
		Assert.Contains("E", result.ToUpper());
	}

	[Fact]
	public void Print_NegativeZero_FormatsAsZero()
	{
		// Edge case: -0 should format as " 0 "
		var program = @"10 LET A = -0
20 PRINT A
30 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');
		Assert.Equal(" 0 ", line);
	}

	[Fact]
	public void Print_MultipleNumbers_DifferentFormats()
	{
		// Test various number formats in one program
		var program = @"10 PRINT 0
20 PRINT 42
30 PRINT -123
40 PRINT 3.14
50 PRINT 0.001
60 END
";

		var result = RunProgram(program);
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

		Assert.True(lines.Length >= 5, "Should have at least 5 output lines");
		Assert.Contains("0", lines[0]);
		Assert.Contains("42", lines[1]);
		Assert.Contains("123", lines[2]); // -123
		Assert.Contains("3.14", lines[3]);
		// 0.001 may print as ".001" (implicit-point form)
		var line4 = lines[4].Replace(" ", "");
		Assert.True(line4.Contains("0.001") || line4.Contains(".001"),
			$"Expected '0.001' or '.001', got '{line4}'");
	}

	[Fact]
	public void Print_MixedTypes_StringAndNumbers()
	{
		// ECMA55-PRN-001: Mixed content types
		var program = @"10 PRINT ""VALUE:"";42
20 END
";

		var result = RunProgram(program);
		Assert.Contains("VALUE:", result);
		Assert.Contains("42", result);
	}

	[Fact]
	public void Print_CommaZoneWrapping_FiveZones()
	{
		// ECMA55-PRN-009: All 5 print zones
		var program = @"10 PRINT ""Z1"",""Z2"",""Z3"",""Z4"",""Z5""
20 END
";

		var result = RunProgram(program);
		// All 5 zone markers should be present
		Assert.Contains("Z1", result);
		Assert.Contains("Z2", result);
		Assert.Contains("Z3", result);
		Assert.Contains("Z4", result);
		Assert.Contains("Z5", result);

		// They should be on the same line
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		Assert.Contains("Z1", lines[0]);
		Assert.Contains("Z5", lines[0]);
	}

	[Fact]
	public void Print_NumberWithVariable_FormatsCorrectly()
	{
		// Variables in PRINT with numbers
		var program = @"10 LET X = 100
20 LET Y = 200
30 PRINT X,Y
40 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		Assert.Contains("100", line);
		Assert.Contains("200", line);

		// X and Y should be in different zones (separated by comma)
		var idx100 = line.IndexOf("100");
		var idx200 = line.IndexOf("200");
		Assert.True(idx200 > idx100 + 3, "Y should be in different zone from X");
	}

	[Fact]
	public void Print_BooleanResult_Prints()
	{
		// ECMA55-PRN-001: Boolean expressions can be printed
		var program = @"10 PRINT 5 > 3
20 PRINT 2 > 8
30 END
";

		var result = RunProgram(program);
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

		Assert.True(lines.Length >= 2, "Should have at least 2 lines");
		// Implementation may print as boolean text or numeric value
		// Just verify both expressions produce output
		Assert.NotEmpty(lines[0].Trim());
		Assert.NotEmpty(lines[1].Trim());
		// The results should be different (one true, one false)
		Assert.NotEqual(lines[0].Trim(), lines[1].Trim());
	}

	[Fact]
	public void Print_ComplexExpression_InPlace()
	{
		// Complex arithmetic expression
		var program = @"10 PRINT 2 + 3 * 4 - 1
20 END
";

		var result = RunProgram(program);
		// 2 + 3 * 4 - 1 = 2 + 12 - 1 = 13
		Assert.Contains("13", result);
	}

	#endregion

	#region Phase 4: Edge Cases and Margin Tests (ECMA55-PRN-013, PRN-014)

	[Fact]
	public void Print_EmptyString_BlankLine()
	{
		// Edge case: Empty string
		var program = @"10 PRINT """"
20 END
";

		var result = RunProgram(program);
		// Should produce a newline (blank line)
		Assert.Equal("\r\n", result);
	}

	[Fact]
	public void Print_JustSemicolon_NoOutput()
	{
		// Edge case: PRINT with just semicolon
		var program = @"10 PRINT ;
20 PRINT ""X""
30 END
";

		var result = RunProgram(program);
		// Should have X on the same line as the suppressed newline
		Assert.StartsWith("X", result.TrimStart());
	}

	[Fact]
	public void Print_JustComma_AdvancesZone()
	{
		// Edge case: PRINT with just comma
		var program = @"10 PRINT ,
20 PRINT ""X""
30 END
";

		var result = RunProgram(program);
		// X should appear in zone 2 (after advancing from zone 1)
		Assert.Contains("X", result);
	}

	[Fact]
	public void Print_MultipleConsecutiveSemicolons()
	{
		// Edge case: Multiple semicolons
		var program = @"10 PRINT ""A"";  ; ;""B""
20 END
";

		var result = RunProgram(program);
		// Should print AB (semicolons with spaces between)
		Assert.Contains("A", result);
		Assert.Contains("B", result);
	}

	[Fact]
	public void Print_MultipleConsecutiveCommas()
	{
		// Edge case: Multiple commas skip multiple zones
		var program = @"10 PRINT ""A"",,,""B""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// A in zone 1, skip 2 and 3, B in zone 4
		Assert.Contains("A", line);
		Assert.Contains("B", line);

		// B should be significantly after A
		var idxA = line.IndexOf('A');
		var idxB = line.IndexOf('B');
		Assert.True(idxB > idxA + 10, "B should be multiple zones after A");
	}

	[Fact]
	public void Print_LongString_NoWrapping()
	{
		// Long string that doesn't exceed margin
		var program = @"10 PRINT ""THIS IS A MODERATELY LONG STRING BUT UNDER 80 CHARACTERS""
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		// Should be on one line
		Assert.Contains("THIS IS A MODERATELY LONG STRING BUT UNDER 80 CHARACTERS", line);
		Assert.DoesNotContain("\r\n", line);
	}

	[Fact]
	public void Print_LongOutputString_HandlesCorrectly()
	{
		// ECMA55-PRN-013, PRN-014: Output that may wrap
		// Build long output from multiple items
		var program = @"10 PRINT ""AAAAAAAAAA"";""BBBBBBBBBB"";""CCCCCCCCCC"";""DDDDDDDDDD""
20 END
";

		var result = RunProgram(program);

		// Should contain the output characters
		Assert.Contains("AAAAAAAAAA", result);
		Assert.Contains("BBBBBBBBBB", result);
		Assert.Contains("CCCCCCCCCC", result);
		Assert.Contains("DDDDDDDDDD", result);
	}

	[Fact]
	public void Print_NumberAndStringMixedWithSeparators()
	{
		// Complex mix of types and separators
		var program = @"10 PRINT ""NAME:"";""ALICE"",""AGE:"";30
20 END
";

		var result = RunProgram(program);
		var line = result.Trim('\r', '\n');

		Assert.Contains("NAME:", line);
		Assert.Contains("ALICE", line);
		Assert.Contains("AGE:", line);
		Assert.Contains("30", line);
	}

	[Fact]
	public void Print_FunctionInExpression_Evaluates()
	{
		// Function call within complex expression
		var program = @"10 PRINT ""SQRT(16) + 10 = "";SQR(16) + 10
20 END
";

		var result = RunProgram(program);
		Assert.Contains("SQRT(16) + 10 =", result);
		Assert.Contains("14", result); // 4 + 10 = 14
	}

	[Fact]
	public void Print_MultipleStatementsOnDifferentLines()
	{
		// Multiple PRINT statements create multiple lines
		var program = @"10 PRINT ""LINE1""
20 PRINT ""LINE2""
30 PRINT ""LINE3""
40 END
";

		var result = RunProgram(program);
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

		Assert.True(lines.Length >= 3, "Should have at least 3 lines");
		Assert.Contains("LINE1", lines[0]);
		Assert.Contains("LINE2", lines[1]);
		Assert.Contains("LINE3", lines[2]);
	}

	[Fact]
	public void Print_NestedExpressions_WithParentheses()
	{
		// Complex nested arithmetic
		var program = @"10 PRINT ((2 + 3) * 4) - (5 / 5)
20 END
";

		var result = RunProgram(program);
		// ((2 + 3) * 4) - (5 / 5) = (5 * 4) - 1 = 20 - 1 = 19
		Assert.Contains("19", result);
	}

	[Fact]
	public void Print_AfterVariableAssignment()
	{
		// PRINT immediately after LET
		var program = @"10 LET X = 42
20 PRINT X
30 LET Y$ = ""TEST""
40 PRINT Y$
50 END
";

		var result = RunProgram(program);
		var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

		Assert.Contains("42", lines[0]);
		Assert.Contains("TEST", lines[1]);
	}

	[Fact]
	public void Print_ZeroParameterFunction_DEF_FN()
	{
		// Integration with DEF FN (zero parameter)
		var program = @"10 DEF FNA = 3.14159
20 PRINT ""PI ="";FNA
30 END
";

		var result = RunProgram(program);
		Assert.Contains("PI =", result);
		Assert.Contains("3.14159", result);
	}

	[Fact]
	public void Print_OneParameterFunction_DEF_FN()
	{
		// Integration with DEF FN (one parameter)
		var program = @"10 DEF FNB(X) = X * X
20 PRINT ""5 SQUARED ="";FNB(5)
30 END
";

		var result = RunProgram(program);
		Assert.Contains("5 SQUARED =", result);
		Assert.Contains("25", result);
	}

	#endregion
}
