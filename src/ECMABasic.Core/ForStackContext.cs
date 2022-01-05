using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public class ForStackContext : ICallStackContext
	{
		public ForStackContext(VariableExpression loopVar, double to, double step, int returnToLineNumber)
		{
			if (!loopVar.IsNumeric)
			{
				throw ExceptionFactory.ExpectedNumericVariable();
			}
			LoopVar = loopVar;
			To = to;
			Step = step;
			BlockStartLineNumber = returnToLineNumber;
			BlockEndLineNumber = null;
		}

		public VariableExpression LoopVar { get; }
		public double To { get; }
		public double Step { get; }

		/// <summary>
		/// Return to this line number when the NEXT is hit.
		/// </summary>
		public int BlockStartLineNumber { get; }

		/// <summary>
		/// The line is one past the closing NEXT block, and isn't defined until NEXT is hit the first time.
		/// </summary>
		public int? BlockEndLineNumber { get; set; }
	}
}
