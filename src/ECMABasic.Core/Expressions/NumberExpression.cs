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

		/// <summary>
		/// Get a new expression that negates this one.
		/// </summary>
		/// <returns>The new number expression.</returns>
		public NumberExpression Negate()
		{
			return new NumberExpression(-Value);
		}

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
