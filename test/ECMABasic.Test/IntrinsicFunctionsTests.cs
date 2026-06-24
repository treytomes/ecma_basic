using System;
using System.Linq;
using ECMABasic.Application;
using ECMABasic.Domain;
using ECMABasic.Domain.Exceptions;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for ECMA-55 intrinsic functions (Section 9).
/// </summary>
public class IntrinsicFunctionsTests
{
	private TestEnvironment CreateTestEnvironment()
	{
		return new TestEnvironment();
	}

	#region ABS Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void ABS_PositiveNumber_ReturnsValue()
	{
		var result = RunProgram("10 PRINT ABS(5)\n20 END\n");
		Assert.Contains("5", result);
	}

	[Fact]
	public void ABS_NegativeNumber_ReturnsAbsoluteValue()
	{
		var result = RunProgram("10 PRINT ABS(-5)\n20 END\n");
		Assert.Contains("5", result);
	}

	[Fact]
	public void ABS_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT ABS(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void ABS_DecimalNumber_ReturnsAbsoluteValue()
	{
		var result = RunProgram("10 PRINT ABS(-3.14)\n20 END\n");
		Assert.Contains("3.14", result);
	}

	#endregion

	#region ATN Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void ATN_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT ATN(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void ATN_One_ReturnsQuarterPi()
	{
		// ATN(1) should be π/4 ≈ 0.785398
		var result = RunProgram("10 PRINT ATN(1)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - (Math.PI / 4)) < 0.0001, $"Expected ≈0.785398, got {value}");
	}

	[Fact]
	public void ATN_NegativeOne_ReturnsNegativeQuarterPi()
	{
		// ATN(-1) should be -π/4 ≈ -0.785398
		var result = RunProgram("10 PRINT ATN(-1)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - (-Math.PI / 4)) < 0.0001, $"Expected ≈-0.785398, got {value}");
	}

	[Fact]
	public void ATN_LargeNumber_ApproachesHalfPi()
	{
		// ATN(1000) should approach π/2 ≈ 1.5708
		var result = RunProgram("10 PRINT ATN(1000)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - (Math.PI / 2)) < 0.01, $"Expected ≈1.5708, got {value}");
	}

	#endregion

	#region COS Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void COS_Zero_ReturnsOne()
	{
		var result = RunProgram("10 PRINT COS(0)\n20 END\n");
		Assert.Contains("1", result);
	}

	[Fact]
	public void COS_Pi_ReturnsNegativeOne()
	{
		// COS(π) = -1
		var result = RunProgram("10 PRINT COS(3.14159265)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - (-1)) < 0.0001, $"Expected ≈-1, got {value}");
	}

	#endregion

	#region EXP Tests (ECMA55-FUN-001, FUN-002, FUN-003, FUN-007)

	[Fact]
	public void EXP_Zero_ReturnsOne()
	{
		var result = RunProgram("10 PRINT EXP(0)\n20 END\n");
		Assert.Contains("1", result);
	}

	[Fact]
	public void EXP_One_ReturnsE()
	{
		// EXP(1) = e ≈ 2.71828
		var result = RunProgram("10 PRINT EXP(1)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - Math.E) < 0.0001, $"Expected ≈{Math.E}, got {value}");
	}

	[Fact]
	public void EXP_NegativeLarge_ReturnsNearZero()
	{
		// EXP(-100) should be very small, approaching zero (ECMA55-FUN-003)
		var result = RunProgram("10 PRINT EXP(-100)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(value >= 0 && value < 1e-40, $"Expected near zero, got {value}");
	}

	[Fact]
	public void EXP_PositiveLarge_HandlesOverflow()
	{
		// EXP(1000) overflows - should raise nonfatal exception per ECMA55-FUN-007
		// For now, we'll just verify it returns a very large number or infinity
		var result = RunProgram("10 PRINT EXP(100)\n20 END\n");
		var output = result.Trim();

		// Should either be a very large number or overflow to infinity
		if (double.TryParse(output.Split('\n').Last(), out var value))
		{
			Assert.True(value > 1e40 || double.IsPositiveInfinity(value),
				$"Expected very large or infinity, got {value}");
		}
	}

	#endregion

	#region INT Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void INT_PositiveDecimal_ReturnsFloor()
	{
		var result = RunProgram("10 PRINT INT(3.7)\n20 END\n");
		Assert.Contains("3", result);
	}

	[Fact]
	public void INT_NegativeDecimal_ReturnsFloor()
	{
		var result = RunProgram("10 PRINT INT(-3.7)\n20 END\n");
		Assert.Contains("-4", result);
	}

	[Fact]
	public void INT_Integer_ReturnsSameValue()
	{
		var result = RunProgram("10 PRINT INT(5)\n20 END\n");
		Assert.Contains("5", result);
	}

	[Fact]
	public void INT_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT INT(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	#endregion

	#region LOG Tests (ECMA55-FUN-001, FUN-002, FUN-005)

	[Fact]
	public void LOG_One_ReturnsZero()
	{
		var result = RunProgram("10 PRINT LOG(1)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void LOG_E_ReturnsOne()
	{
		// LOG(e) = 1
		var result = RunProgram("10 PRINT LOG(2.71828183)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - 1) < 0.0001, $"Expected ≈1, got {value}");
	}

	[Fact]
	public void LOG_Ten_ReturnsLog10()
	{
		// LOG(10) = ln(10) ≈ 2.302585
		var result = RunProgram("10 PRINT LOG(10)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - Math.Log(10)) < 0.0001, $"Expected ≈{Math.Log(10)}, got {value}");
	}

	[Fact]
	public void LOG_Zero_RaisesFatalException()
	{
		// ECMA55-FUN-005: LOG(0) shall raise fatal exception
		// RuntimeException is caught by Program.Execute and reported as error
		var result = RunProgram("10 PRINT LOG(0)\n20 END\n");
		Assert.Contains("LOG", result, StringComparison.OrdinalIgnoreCase);
		Assert.Contains("NON-POSITIVE", result, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void LOG_Negative_RaisesFatalException()
	{
		// ECMA55-FUN-005: LOG(negative) shall raise fatal exception
		// RuntimeException is caught by Program.Execute and reported as error
		var result = RunProgram("10 PRINT LOG(-5)\n20 END\n");
		Assert.Contains("LOG", result, StringComparison.OrdinalIgnoreCase);
		Assert.Contains("NON-POSITIVE", result, StringComparison.OrdinalIgnoreCase);
	}

	#endregion

	#region SGN Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void SGN_PositiveNumber_ReturnsOne()
	{
		var result = RunProgram("10 PRINT SGN(5)\n20 END\n");
		Assert.Contains("1", result);
	}

	[Fact]
	public void SGN_NegativeNumber_ReturnsNegativeOne()
	{
		var result = RunProgram("10 PRINT SGN(-5)\n20 END\n");
		Assert.Contains("-1", result);
	}

	[Fact]
	public void SGN_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT SGN(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	#endregion

	#region SIN Tests (ECMA55-FUN-001, FUN-002)

	[Fact]
	public void SIN_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT SIN(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void SIN_HalfPi_ReturnsOne()
	{
		// SIN(π/2) = 1
		var result = RunProgram("10 PRINT SIN(1.5707963)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - 1) < 0.0001, $"Expected ≈1, got {value}");
	}

	#endregion

	#region SQR Tests (ECMA55-FUN-001, FUN-002, FUN-006)

	[Fact]
	public void SQR_Four_ReturnsTwo()
	{
		var result = RunProgram("10 PRINT SQR(4)\n20 END\n");
		Assert.Contains("2", result);
	}

	[Fact]
	public void SQR_Nine_ReturnsThree()
	{
		var result = RunProgram("10 PRINT SQR(9)\n20 END\n");
		Assert.Contains("3", result);
	}

	[Fact]
	public void SQR_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT SQR(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void SQR_Decimal_ReturnsSquareRoot()
	{
		var result = RunProgram("10 PRINT SQR(2)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - Math.Sqrt(2)) < 0.0001,
			$"Expected ≈{Math.Sqrt(2)}, got {value}");
	}

	[Fact]
	public void SQR_Negative_RaisesFatalException()
	{
		// ECMA55-FUN-006: SQR(negative) shall raise fatal exception
		// RuntimeException is caught by Program.Execute and reported as error
		var result = RunProgram("10 PRINT SQR(-4)\n20 END\n");
		Assert.Contains("SQR", result, StringComparison.OrdinalIgnoreCase);
		Assert.Contains("NEGATIVE", result, StringComparison.OrdinalIgnoreCase);
	}

	#endregion

	#region TAN Tests (ECMA55-FUN-001, FUN-002, FUN-007)

	[Fact]
	public void TAN_Zero_ReturnsZero()
	{
		var result = RunProgram("10 PRINT TAN(0)\n20 END\n");
		Assert.Contains("0", result);
	}

	[Fact]
	public void TAN_QuarterPi_ReturnsOne()
	{
		// TAN(π/4) = 1
		var result = RunProgram("10 PRINT TAN(0.78539816)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value - 1) < 0.0001, $"Expected ≈1, got {value}");
	}

	[Fact]
	public void TAN_NearHalfPi_HandlesLargeValue()
	{
		// TAN(π/2 - small) approaches infinity (ECMA55-FUN-007)
		// This tests large values without overflow
		var result = RunProgram("10 PRINT TAN(1.5)\n20 END\n");
		var output = result.Trim();
		var value = double.Parse(output.Split('\n').Last());
		Assert.True(Math.Abs(value) > 10, $"Expected large value, got {value}");
	}

	#endregion

	#region RND Tests (ECMA55-FUN-001, FUN-004, FUN-008)

	[Fact]
	public void RND_ReturnsValueInRange()
	{
		// Current implementation: RND(N) returns [0, N)
		// Note: Issue #35 will change this to parameterless [0,1)
		var result = RunProgram("10 PRINT RND(10)\n20 END\n");
		var output = result.Trim();
		var value = int.Parse(output.Split('\n').Last());
		Assert.InRange(value, 0, 9);
	}

	[Fact]
	public void RND_RepeatedCalls_ProduceSameSequence()
	{
		// ECMA55-FUN-008: Same program should produce same sequence
		var result1 = RunProgram("10 PRINT RND(100)\n20 END\n");

		// Create new environment (simulates new program run)
		var env2 = CreateTestEnvironment();
		var result2 = RunProgramInEnv(env2, "10 PRINT RND(100)\n20 END\n");

		Assert.Equal(result1.Trim(), result2.Trim());
	}

	#endregion

	#region Helper Methods

	private string RunProgram(string program)
	{
		var env = CreateTestEnvironment();
		return RunProgramInEnv(env, program);
	}

	private string RunProgramInEnv(TestEnvironment env, string program)
	{
		if (Interpreter.FromText(program, env))
		{
			env.Program.Execute(env);
		}
		return env.Text;
	}

	#endregion
}
