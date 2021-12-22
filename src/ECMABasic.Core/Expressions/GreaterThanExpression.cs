using System;

namespace ECMABasic.Core.Expressions
{
	public class GreaterThanExpression : BooleanExpression
	{
		public GreaterThanExpression(IExpression left, IExpression right)
			: base(left, right)
		{
		}

		public override object Evaluate(IEnvironment env)
		{
			var left = Left.Evaluate(env);
			var right = Right.Evaluate(env);
			return (left as IComparable).CompareTo(right) > 0;
		}

		public override string ToListing()
		{
			return string.Concat(Left.ToListing(), ">", Right.ToListing());
		}
	}
}
