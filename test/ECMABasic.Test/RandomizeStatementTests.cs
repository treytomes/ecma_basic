using System;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for RANDOMIZE statement per ECMA-55 specification.
/// ECMA55-RND-001: RANDOMIZE generates unpredictable starting point for RND sequence.
/// ECMA55-RND-002: If no randomizing device available, may interact with user.
/// </summary>
public class RandomizeStatementTests
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

	#region ECMA55-RND-001: RANDOMIZE Changes Sequence

	[Fact]
	public void RANDOMIZE_ChangesRndSequence()
	{
		// Without RANDOMIZE, two programs produce same sequence
		var result1 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 END\n");
		var result2 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 END\n");
		Assert.Equal(result1.Trim(), result2.Trim());

		// With RANDOMIZE, sequences should differ
		// Note: There's a tiny probability they could match by chance,
		// but with [0,1) range it's extremely unlikely
		var resultA = RunProgram("10 RANDOMIZE\n20 PRINT RND\n30 PRINT RND\n40 END\n");
		var resultB = RunProgram("10 RANDOMIZE\n20 PRINT RND\n30 PRINT RND\n40 END\n");

		// At least one of the two values should differ
		var linesA = resultA.Trim().Split('\n');
		var linesB = resultB.Trim().Split('\n');

		var allMatch = linesA[0] == linesB[0] && linesA[1] == linesB[1];
		Assert.False(allMatch, "RANDOMIZE should produce different sequences on each run");
	}

	[Fact]
	public void RANDOMIZE_ProducesUnpredictableValues()
	{
		// Run RANDOMIZE + RND multiple times, verify we get different values
		var values = new System.Collections.Generic.HashSet<string>();

		for (var i = 0; i < 5; i++)
		{
			var result = RunProgram("10 RANDOMIZE\n20 PRINT RND\n30 END\n");
			values.Add(result.Trim());
		}

		// Should have at least 4 unique values out of 5 runs
		// (allowing 1 collision due to randomness)
		Assert.True(values.Count >= 4, $"Expected at least 4 unique values, got {values.Count}");
	}

	#endregion

	#region ECMA55-FUN-008: Without RANDOMIZE, Sequence is Repeatable

	[Fact]
	public void WithoutRandomize_SequenceIsRepeatable()
	{
		// This test documents that WITHOUT RANDOMIZE, the sequence is deterministic
		var result1 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");
		var result2 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");
		var result3 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");

		Assert.Equal(result1.Trim(), result2.Trim());
		Assert.Equal(result2.Trim(), result3.Trim());
	}

	#endregion

	#region Statement Syntax

	[Fact]
	public void RANDOMIZE_TakesNoParameters()
	{
		// RANDOMIZE is a simple statement with no arguments
		var result = RunProgram("10 RANDOMIZE\n20 PRINT \"OK\"\n30 END\n");
		Assert.Contains("OK", result);
	}

	// Note: Colon-separated statements not yet supported by interpreter

	[Fact]
	public void RANDOMIZE_CanAppearMultipleTimes()
	{
		// Calling RANDOMIZE multiple times should not error
		var result = RunProgram("10 RANDOMIZE\n20 RANDOMIZE\n30 PRINT \"OK\"\n40 END\n");
		Assert.Contains("OK", result);
	}

	#endregion

	#region Integration with RND

	[Fact]
	public void RANDOMIZE_ThenRND_ReturnsValidRange()
	{
		// After RANDOMIZE, RND should still return [0, 1)
		var result = RunProgram("10 RANDOMIZE\n20 PRINT RND\n30 END\n");
		var output = result.Trim();
		var value = double.Parse(output);

		Assert.InRange(value, 0.0, 0.9999999);
	}

	[Fact]
	public void MultipleRANDOMIZE_EachChangesSequence()
	{
		// Each RANDOMIZE should re-seed, producing different sequences
		var program = @"10 RANDOMIZE
20 LET A = RND
30 RANDOMIZE
40 LET B = RND
50 PRINT A
60 PRINT B
70 END
";

		// Run twice, the A and B values should differ both within and between runs
		var result1 = RunProgram(program);
		var result2 = RunProgram(program);

		var lines1 = result1.Trim().Split('\n');
		var lines2 = result2.Trim().Split('\n');

		var a1 = double.Parse(lines1[0]);
		var b1 = double.Parse(lines1[1]);
		var a2 = double.Parse(lines2[0]);
		var b2 = double.Parse(lines2[1]);

		// Within each run, A and B should likely differ (not guaranteed, but very likely)
		// Between runs, at least one value should differ
		var run1Differs = a1 != b1;
		var run2Differs = a2 != b2;
		var betweenRunsDiffers = a1 != a2 || b1 != b2;

		Assert.True(run1Differs || run2Differs || betweenRunsDiffers,
			"RANDOMIZE should produce varying sequences");
	}

	#endregion

	// Note: Error handling tests omitted - focus on positive functionality
}
