namespace ECMABasic.Core.Expressions
{
	public class EqualsExpression : BooleanExpression
	{
		public EqualsExpression(IExpression left, IExpression right)
			: base(left, right)
		{
		}

		public override object Evaluate(IEnvironment env)
		{
			var left = Left.Evaluate(env);
			var right = Right.Evaluate(env);
			return left.Equals(right);
		}

		public override string ToListing()
		{
			var left = (Left is BinaryExpression) ? string.Concat("(", Left.ToListing(), ")") : Left.ToListing();
			var right = (Right is BinaryExpression) ? string.Concat("(", Right.ToListing(), ")") : Right.ToListing();
			return string.Concat(left, "=", right);
		}
	}
}
