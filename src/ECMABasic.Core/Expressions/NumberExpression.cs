namespace ECMABasic.Core.Expressions
{
	public class NumberExpression : IExpression
	{
		public NumberExpression(double value)
		{
			Value = value;
		}

		public double Value { get; }

		public object Evaluate(IEnvironment env)
		{
			return Value;
		}
	}
}
