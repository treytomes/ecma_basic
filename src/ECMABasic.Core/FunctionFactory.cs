using System;
using System.Collections.Generic;

namespace ECMABasic.Core
{
	// TODO: This really needs to be part of the environment rather than a global singleton.

	public class FunctionFactory
	{
		private readonly List<FunctionDefinition> _functions = new();

		static FunctionFactory()
		{
			Instance = new();
		}

		// TODO: Built-in functions: ATN, EXP, LOG, SQR
		// DONE: Built-in functions: ABS, COS, INT, RND, SGN, SIN, TAN

		// TODO: What are the rules for RND?  Is the 1 parameter inclusive or exclusive?  What is the lower bound?

		private FunctionFactory()
		{
			Define("ABS", new[] { ExpressionType.Number }, args => Math.Abs(Convert.ToDouble(args[0])));
			Define("COS", new[] { ExpressionType.Number }, args => Math.Cos(Convert.ToDouble(args[0])));
			Define("INT", new[] { ExpressionType.Number }, args => Convert.ToInt32(args[0]));
			Define("RND", new[] { ExpressionType.Number }, args => RandomFactory.Instance.Next(Convert.ToInt32(args[0])));
			Define("SGN", new[] { ExpressionType.Number }, args => Math.Sign(Convert.ToDouble(args[0])));
			Define("SIN", new[] { ExpressionType.Number }, args => Math.Sin(Convert.ToDouble(args[0])));
			Define("TAN", new[] { ExpressionType.Number }, args => Math.Tan(Convert.ToDouble(args[0])));
		}

		public static FunctionFactory Instance { get; }

		public void Define(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn)
		{
			_functions.Add(new FunctionDefinition(name, args, fn));
		}

		/// <summary>
		/// Get all functions with the given name.
		/// </summary>
		/// <param name="name">The function name to look for.</param>
		/// <returns>A list of functions with the given name.</returns>
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
}
