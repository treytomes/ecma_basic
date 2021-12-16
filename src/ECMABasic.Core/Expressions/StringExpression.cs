namespace ECMABasic.Core.Expressions
{
	class StringExpression : IExpression
	{
		public StringExpression(string text)
			: base()
		{
			Text = text;
		}

		public string Text { get; }

		public object Evaluate(IEnvironment env)
		{
			return Text;
		}

		public string ToListing()
		{
			return string.Concat("\"", Text, "\"");
		}
	}
}
