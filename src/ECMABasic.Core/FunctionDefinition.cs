using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	public class FunctionDefinition
	{
		private readonly string _name;
		private readonly List<ExpressionType> _arguments;
		private readonly Func<List<object>, object> _fn;

		public FunctionDefinition(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn)
		{
			_name = name;
			_arguments = new List<ExpressionType>(args);
			_fn = fn;
		}

		public FunctionExpression Instantiate(IEnumerable<IExpression> args, int? lineNumber = null)
		{
			if (args.Count() != _arguments.Count)
			{
				throw ExceptionFactory.ArgumentCountMismatch(lineNumber);
			}
			for (var n = 0; n < _arguments.Count; n++)
			{
				if (args.ElementAt(n).Type != _arguments[n])
				{
					throw ExceptionFactory.ArgumentTypeMismatch(lineNumber);
				}
			}
			return new FunctionExpression(_name, _fn, args);
		}
	}
}
