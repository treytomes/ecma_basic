using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public class ForStackContext : ICallStackContext
	{
		public ForStackContext(VariableExpression loopVar, IExpression to, IExpression step, int returnToLineNumber)
		{
			if (!loopVar.IsNumeric)
			{
				throw ExceptionFactory.ExpectedNumericVariable();
			}
			LoopVar = loopVar;
			To = to;
			Step = step;
			LineNumber = returnToLineNumber;
		}

		public VariableExpression LoopVar { get; }
		public IExpression To { get; }
		public IExpression Step { get; }

		/// <summary>
		/// Return to this line number when the NEXT is hit.
		/// </summary>
		public int LineNumber { get; }
	}
}
