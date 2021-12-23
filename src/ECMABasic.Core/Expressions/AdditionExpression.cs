using System;

namespace ECMABasic.Core.Expressions
{
	public class AdditionExpression : BinaryExpression
	{
		public AdditionExpression(IExpression left, IExpression right)
			: base(left, right)
		{
		}

		public override object Evaluate(IEnvironment env)
		{
			var left = Convert.ToDouble(Left.Evaluate(env));
			var right = Convert.ToDouble(Right.Evaluate(env));
			return left + right;
		}

		public override string ToListing()
		{
			return string.Concat(Left.ToListing(), " + ", Right.ToListing());
		}
	}
}
