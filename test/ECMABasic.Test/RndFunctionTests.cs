using System;
using System.Linq;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for ECMA-55 compliant RND function (Issue #35).
/// ECMA55-FUN-004: RND shall return uniformly distributed value in [0, 1).
/// ECMA55-FUN-008: Without RANDOMIZE, same program produces same sequence.
/// </summary>
public class RndFunctionTests
{
	#region ECMA55-FUN-004: RND Returns [0, 1)

	[Fact]
	public void RND_TakesNoParameters()
	{
		// Arrange & Act
		var result = RunProgram("10 PRINT RND\n20 END\n");

		// Assert - Should compile and execute without error
		Assert.NotEmpty(result);
	}

	[Fact]
	public void RND_ReturnsDoubleValue()
	{
		// Arrange & Act
		var result = RunProgram("10 PRINT RND\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());

		// Assert - Should be a valid double
		Assert.True(double.IsFinite(value));
	}

	[Fact]
	public void RND_ReturnsValueInRangeZeroToOne()
	{
		// Arrange & Act - Generate multiple random numbers
		var result = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 PRINT RND\n50 PRINT RND\n60 END\n");
		var lines = result.Trim().Split('\n');

		// Assert - All values should be in [0, 1)
		foreach (var line in lines)
		{
			if (double.TryParse(line.Trim(), out var value))
			{
				Assert.True(value >= 0.0, $"RND returned {value}, which is less than 0");
				Assert.True(value < 1.0, $"RND returned {value}, which is >= 1");
			}
		}
	}

	[Fact]
	public void RND_ReturnsZeroOrGreater()
	{
		// Arrange & Act - Get a single RND value
		var result = RunProgram("10 PRINT RND\n20 END\n");
		var value = double.Parse(result.Trim().Split('\n').Last());

		// Assert
		Assert.True(value >= 0.0, $"RND must return value >= 0, got {value}");
	}

	[Fact]
	public void RND_ReturnsLessThanOne()
	{
		// Arrange & Act - Get a single RND value
		var result = RunProgram("10 PRINT RND\n20 END\n");
		var value = double.Parse(result.Trim().Split('\n').Last());

		// Assert
		Assert.True(value < 1.0, $"RND must return value < 1, got {value}");
	}

	[Fact]
	public void RND_MultipleCallsReturnDifferentValues()
	{
		// Arrange & Act
		var result = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");
		var lines = result.Trim().Split('\n');
		var values = lines.Select(line => double.Parse(line.Trim())).ToList();

		// Assert - At least some values should be different (extremely unlikely all 3 are same)
		Assert.True(values.Distinct().Count() > 1,
			"RND should produce different values on repeated calls");
	}

	#endregion

	#region ECMA55-FUN-008: Repeatability Without RANDOMIZE

	[Fact]
	public void RND_WithoutRandomize_ProducesSameSequence()
	{
		// ECMA55-FUN-008: Same program should produce same sequence
		// Arrange - Run program twice in fresh environments
		var result1 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");
		var result2 = RunProgram("10 PRINT RND\n20 PRINT RND\n30 PRINT RND\n40 END\n");

		// Assert - Both runs should produce identical output
		Assert.Equal(result1.Trim(), result2.Trim());
	}

	[Fact]
	public void RND_FirstValueIsAlwaysSame()
	{
		// Arrange & Act - Get first RND value from multiple runs
		var result1 = RunProgram("10 PRINT RND\n20 END\n");
		var result2 = RunProgram("10 PRINT RND\n20 END\n");
		var result3 = RunProgram("10 PRINT RND\n20 END\n");

		var value1 = double.Parse(result1.Trim().Split('\n').Last());
		var value2 = double.Parse(result2.Trim().Split('\n').Last());
		var value3 = double.Parse(result3.Trim().Split('\n').Last());

		// Assert - All first values should be identical (fixed seed)
		Assert.Equal(value1, value2);
		Assert.Equal(value2, value3);
	}

	[Fact]
	public void RND_TenCallsProduceSameSequence()
	{
		// Arrange - Generate 10 random numbers in two separate runs
		var program = "10 FOR I = 1 TO 10\n20 PRINT RND\n30 NEXT I\n40 END\n";
		var result1 = RunProgram(program);
		var result2 = RunProgram(program);

		// Assert - Both sequences should be identical
		Assert.Equal(result1.Trim(), result2.Trim());
	}

	#endregion

	#region Distribution Tests (Quality Check)

	[Fact]
	public void RND_HundredCalls_SpreadAcrossRange()
	{
		// Arrange - Generate 100 random numbers
		var program = "10 FOR I = 1 TO 100\n20 PRINT RND\n30 NEXT I\n40 END\n";
		var result = RunProgram(program);
		var lines = result.Trim().Split('\n');
		var values = lines.Select(line => double.Parse(line.Trim())).ToList();

		// Assert - Values should cover different parts of [0, 1)
		var lowValues = values.Count(v => v < 0.25);   // [0, 0.25)
		var midLowValues = values.Count(v => v >= 0.25 && v < 0.5);  // [0.25, 0.5)
		var midHighValues = values.Count(v => v >= 0.5 && v < 0.75); // [0.5, 0.75)
		var highValues = values.Count(v => v >= 0.75); // [0.75, 1)

		// We expect roughly 25 in each quartile (with some variance)
		// Using loose bounds: at least 10 in each quartile
		Assert.True(lowValues >= 10, $"Expected at least 10 values in [0, 0.25), got {lowValues}");
		Assert.True(midLowValues >= 10, $"Expected at least 10 values in [0.25, 0.5), got {midLowValues}");
		Assert.True(midHighValues >= 10, $"Expected at least 10 values in [0.5, 0.75), got {midHighValues}");
		Assert.True(highValues >= 10, $"Expected at least 10 values in [0.75, 1), got {highValues}");
	}

	#endregion

	// Note: RND(N) syntax with parameter is no longer supported per ECMA-55.
	// The parser will accept it but the intrinsic registry will reject it
	// with an "UNDEFINED FUNCTION" error since RND takes no parameters.

	#region Helper Methods

	private string RunProgram(string program)
	{
		var env = new TestEnvironment();
		Interpreter.FromText(program, env);
		env.Program.Execute(env);
		return env.Text;
	}

	#endregion
}
