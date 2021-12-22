namespace ECMABasic.Core
{
	public interface IExpression : IPrintItem
	{
		ExpressionType Type { get; }
	}
}
