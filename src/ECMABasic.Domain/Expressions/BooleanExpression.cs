namespace ECMABasic.Domain.Expressions;

public abstract class BooleanExpression : BinaryExpression
{
	protected BooleanExpression(IExpression left, IExpression right)
		: base(left, right)
	{
	}
}
