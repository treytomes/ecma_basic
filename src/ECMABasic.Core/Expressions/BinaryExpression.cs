using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Expressions
{
	public abstract class BinaryExpression : IExpression
	{
		protected BinaryExpression(IExpression left, IExpression right)
		{
			Left = left;
			Right = right;

			if (Left.Type != Right.Type)
			{
				throw ExceptionFactory.MixedStringsAndNumbers();
			}
		}

		public IExpression Left { get; }
		public IExpression Right { get; }

		public ExpressionType Type => Left.Type;

		public abstract object Evaluate(IEnvironment env);
		public abstract string ToListing();
	}
}
