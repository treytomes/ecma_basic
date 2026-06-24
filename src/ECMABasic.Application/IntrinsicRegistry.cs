using System;
using System.Collections.Generic;
using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;

namespace ECMABasic.Application;

/// <summary>
/// Registry for ECMA-55 intrinsic functions (built-in functions).
/// Each environment instance gets its own registry for test isolation and future dialect support.
/// </summary>
public class IntrinsicRegistry : IIntrinsicRegistry
{
	private readonly List<FunctionDefinition> _functions = new();

	// TODO: Built-in functions: ATN, EXP, LOG, SQR (Issue #34)
	// DONE: Built-in functions: ABS, COS, INT, RND, SGN, SIN, TAN
	// TODO: Fix RND to be parameterless and return [0,1) per ECMA-55 (Issue #35)

	public IntrinsicRegistry()
	{
		RegisterBuiltInIntrinsics();
	}

	/// <summary>
	/// Register ECMA-55 required intrinsic functions.
	/// </summary>
	private void RegisterBuiltInIntrinsics()
	{
		Register("ABS", [ExpressionType.Number], (env, args) => Math.Abs(Convert.ToDouble(args[0])));
		Register("COS", [ExpressionType.Number], (env, args) => Math.Cos(Convert.ToDouble(args[0])));
		Register("INT", [ExpressionType.Number], (env, args) => Convert.ToInt32(args[0]));
		Register("RND", [ExpressionType.Number], (env, args) => env.Random.Next(Convert.ToInt32(args[0])));
		Register("SGN", [ExpressionType.Number], (env, args) => Math.Sign(Convert.ToDouble(args[0])));
		Register("SIN", [ExpressionType.Number], (env, args) => Math.Sin(Convert.ToDouble(args[0])));
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
