using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	public class FunctionDefinition
	{
		private readonly List<ExpressionType> _arguments;
		private readonly Func<List<object>, object> _fn;

		public FunctionDefinition(string name, IEnumerable<ExpressionType> args, Func<List<object>, object> fn)
		{
			Name = name;
			_arguments = new List<ExpressionType>(args);
			_fn = fn;
		}

		public string Name { get; }

		public ExpressionType Type
		{
			get
			{
				if (Name.EndsWith("$"))
				{
					return ExpressionType.String;
				}
				else
				{
					return ExpressionType.Number;
				}
			}
		}

		public bool CanInstantiate(IEnumerable<IExpression> args)
		{
			if (args.Count() != _arguments.Count)
			{
				return false;
			}
			for (var n = 0; n < _arguments.Count; n++)
			{
				if (args.ElementAt(n).Type != _arguments[n])
				{
					return false;
				}
			}
			return true;
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
			return new FunctionExpression(Name, _fn, args);
		}
	}
}
