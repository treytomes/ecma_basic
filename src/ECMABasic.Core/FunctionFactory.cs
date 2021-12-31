using System;
using System.Collections.Generic;

namespace ECMABasic.Core
{
	public class FunctionFactory
	{
		private Dictionary<string, FunctionDefinition> _functions = new();

		static FunctionFactory()
		{
			Instance = new();
		}

		private FunctionFactory()
		{
			Define("COS", new[] { ExpressionType.Number }, args => Math.Cos(Convert.ToDouble(args[0])));
		}

		public static FunctionFactory Instance { get; }

		public void Define(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn)
		{
			_functions[name] = new FunctionDefinition(name, args, fn);
		}

		public FunctionDefinition Get(string name)
		{
			if (_functions.ContainsKey(name))
			{
				return _functions[name];
			}
			return null;
		}
	}
}
