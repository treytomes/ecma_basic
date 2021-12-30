using System;

namespace ECMABasic.Core.Expressions
{
	/// <summary>
	/// Involution = a number to a power.
	/// </summary>
	public class InvolutionExpression : BinaryExpression
	{
		public InvolutionExpression(IExpression left, IExpression right)
			: base(left, right)
		{
		}

		public override object Evaluate(IEnvironment env)
		{
			var left = Convert.ToDouble(Left.Evaluate(env));
			var right = Convert.ToDouble(Right.Evaluate(env));
			return Math.Pow(left, right);
		}

		public override string ToListing()
		{
			var left = (Left is BinaryExpression) ? string.Concat("(", Left.ToListing(), ")") : Left.ToListing();
			var right = (Right is BinaryExpression) ? string.Concat("(", Right.ToListing(), ")") : Right.ToListing();
			return string.Concat(left, "^", right);
		}
	}
}
