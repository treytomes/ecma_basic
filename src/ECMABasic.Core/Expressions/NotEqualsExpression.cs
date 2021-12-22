namespace ECMABasic.Core.Expressions
{
	public class NotEqualsExpression : BooleanExpression
	{
		public NotEqualsExpression(IExpression left, IExpression right)
			: base(left, right)
		{
		}

		public override object Evaluate(IEnvironment env)
		{
			var left = Left.Evaluate(env);
			var right = Right.Evaluate(env);
			return !left.Equals(right);
		}

		public override string ToListing()
		{
			return string.Concat(Left.ToListing(), "=", Right.ToListing());
		}
	}
}
