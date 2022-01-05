using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core.Expressions
{
	public class NegationExpression : IExpression
	{
		public NegationExpression(IExpression root)
		{
			if (root.Type != ExpressionType.Number)
			{
				throw ExceptionFactory.ExpectedNumericExpression();
			}
			Root = root;
		}

		public IExpression Root { get; }

		public ExpressionType Type => ExpressionType.Number;

		public bool IsReducible => Root.IsReducible;

		public object Evaluate(IEnvironment env)
		{
			var root = Convert.ToDouble(Root.Evaluate(env));
			return -root;
		}

		public string ToListing()
		{
			var root = (Root is BinaryExpression) ? string.Concat("(", Root.ToListing(), ")") : Root.ToListing();
			return string.Concat("-", root);
		}
	}
}
