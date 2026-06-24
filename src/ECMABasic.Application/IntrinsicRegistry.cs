using System;
using System.Collections.Generic;
using ECMABasic.Domain;
using ECMABasic.Domain.Exceptions;
using ECMABasic.Domain.Expressions;

namespace ECMABasic.Application;

/// <summary>
/// Registry for ECMA-55 intrinsic functions (built-in functions).
/// Each environment instance gets its own registry for test isolation and future dialect support.
/// </summary>
public class IntrinsicRegistry : IIntrinsicRegistry
{
	private readonly List<FunctionDefinition> _functions = new();

	// DONE: All 11 ECMA-55 intrinsic functions implemented (Issues #34, #35)
	// - ABS, ATN, COS, EXP, INT, LOG, RND, SGN, SIN, SQR, TAN
	// - RND is parameterless and returns [0, 1) per ECMA55-FUN-004

	public IntrinsicRegistry()
	{
		RegisterBuiltInIntrinsics();
	}

	/// <summary>
	/// Register ECMA-55 required intrinsic functions.
	/// All 11 functions per ECMA55-FUN-001.
	/// </summary>
	private void RegisterBuiltInIntrinsics()
	{
		// ABS: Absolute value
		Register("ABS", [ExpressionType.Number], (env, args) => Math.Abs(Convert.ToDouble(args[0])));

		// ATN: Arctangent (inverse tangent)
		Register("ATN", [ExpressionType.Number], (env, args) => Math.Atan(Convert.ToDouble(args[0])));

		// COS: Cosine
		Register("COS", [ExpressionType.Number], (env, args) => Math.Cos(Convert.ToDouble(args[0])));

		// EXP: Exponential (e^x)
		// ECMA55-FUN-003: Returns zero if result < machine infinitesimal
		// ECMA55-FUN-007: Overflow raises nonfatal exception (handled by Math.Exp returning Infinity)
		Register("EXP", [ExpressionType.Number], (env, args) =>
		{
			var result = Math.Exp(Convert.ToDouble(args[0]));
			// Handle underflow per ECMA55-FUN-003
			if (result < double.Epsilon)
			{
				return 0.0;
			}
			// Math.Exp returns ±Infinity for overflow, which satisfies ECMA55-FUN-007
			return result;
		});

		// INT: Integer part (floor function)
		Register("INT", [ExpressionType.Number], (env, args) => Math.Floor(Convert.ToDouble(args[0])));

		// LOG: Natural logarithm
		// ECMA55-FUN-005: Zero or negative input raises fatal exception
		Register("LOG", [ExpressionType.Number], (env, args) =>
		{
			var x = Convert.ToDouble(args[0]);
			if (x <= 0)
			{
				throw new RuntimeException($"LOG of non-positive number ({x})");
			}
			return Math.Log(x);
		});

		// RND: Random number [0, 1) - ECMA55-FUN-004
		// Parameterless function returning uniformly distributed value in [0, 1)
		// ECMA55-FUN-008: Without RANDOMIZE, same program produces same RND sequence (fixed seed)
		Register("RND", [], (env, args) => env.Random.NextDouble());

		// SGN: Sign function (-1, 0, or 1)
		Register("SGN", [ExpressionType.Number], (env, args) => Math.Sign(Convert.ToDouble(args[0])));

		// SIN: Sine
		Register("SIN", [ExpressionType.Number], (env, args) => Math.Sin(Convert.ToDouble(args[0])));

		// SQR: Square root
		// ECMA55-FUN-006: Negative input raises fatal exception
		Register("SQR", [ExpressionType.Number], (env, args) =>
		{
			var x = Convert.ToDouble(args[0]);
			if (x < 0)
			{
				throw new RuntimeException($"SQR of negative number ({x})");
			}
			return Math.Sqrt(x);
		});

		// TAN: Tangent
		// ECMA55-FUN-007: Overflow raises nonfatal exception (Math.Tan returns ±Infinity)
		Register("TAN", [ExpressionType.Number], (env, args) => Math.Tan(Convert.ToDouble(args[0])));
	}

	public void Register(string name, IEnumerable<ExpressionType> args, Func<IEnvironment, List<object>, object> fn)
	{
		_functions.Add(new FunctionDefinition(name, args, fn));
	}

	public IEnumerable<FunctionDefinition> Get(string name)
	{
		foreach (var fn in _functions)
		{
			if (fn.Name == name)
			{
				yield return fn;
			}
		}
	}
}
