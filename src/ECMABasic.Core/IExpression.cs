namespace ECMABasic.Core
{
	public interface IExpression : IPrintItem
	{
		/// <summary>
		/// Is it a number, or a string?
		/// </summary>
		ExpressionType Type { get; }

		/// <summary>
		/// Can the expression tree be reduced to a literal?
		/// </summary>
		bool IsReducible { get; }
	}
}
