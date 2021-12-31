using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace ECMABasic.Core.Expressions
{
	public class CosExpression : FunctionExpression
	{
		public CosExpression(IEnumerable<IExpression> args)
			: base(args)
		{
			if (Arguments.Count != 1)
			{
				throw new SyntaxException("ARGUMENT COUNT MISMATCH");
			}

			if (Arguments[0].Type != ExpressionType.Number)
			{
				throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION");
			}
		}

		public override string Name => "COS";

		public override ExpressionType Type => ExpressionType.Number;

		public override object Evaluate(IEnvironment env)
		{
			var arg = Convert.ToDouble(Arguments[0].Evaluate(env));
			return Math.Cos(arg);
		}
	}
}
