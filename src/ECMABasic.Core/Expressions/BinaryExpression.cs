namespace ECMABasic.Core.Expressions
{
	public abstract class BinaryExpression : IExpression
	{
		protected BinaryExpression(IExpression left, IExpression right)
		{
			Left = left;
			Right = right;
		}

		public IExpression Left { get; }
		public IExpression Right { get; }

		public abstract object Evaluate(IEnvironment env);
		public abstract string ToListing();
	}
}
