namespace ECMABasic.Core.Expressions
{
	public class IntegerExpression : IExpression
	{
		public IntegerExpression(int value)
		{
			Value = value;
		}

		public int Value { get; }

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
