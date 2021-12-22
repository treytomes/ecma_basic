namespace ECMABasic.Core.Expressions
{
	public class NumberExpression : IExpression
	{
		public NumberExpression(double value)
		{
			Value = value;
		}

		public double Value { get; }

		public ExpressionType Type => ExpressionType.Number;

		public object Evaluate(IEnvironment env)
		{
			return Value;
		}

		public string ToListing()
		{
			return Value.ToString();
		}
	}
}
