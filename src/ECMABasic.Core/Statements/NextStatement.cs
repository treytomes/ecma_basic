using ECMABasic.Core.Expressions;
using System;

namespace ECMABasic.Core.Statements
{
	public class NextStatement : IStatement
	{
		public NextStatement(VariableExpression loopVar)
		{
			if (!loopVar.IsNumeric)
			{
				throw ExceptionFactory.ExpectedNumericVariable();
			}
			LoopVar = loopVar;
		}

		public VariableExpression LoopVar { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (env.PopCallStack() is not ForStackContext context)
			{
				throw ExceptionFactory.NextWithoutFor(env.CurrentLineNumber);
			}

			if (context.LoopVar.Name != LoopVar.Name)
			{
				throw ExceptionFactory.NextWithoutFor(env.CurrentLineNumber);
			}
			 
			if (!context.BlockEndLineNumber.HasValue)
			{
				var blockEndLineNumber = env.Program.GetNextLineNumber(env.CurrentLineNumber);
				context.BlockEndLineNumber = blockEndLineNumber;
			}

			env.CurrentLineNumber = context.BlockStartLineNumber;
			env.PushCallStack(context);  // We'll stay in the FOR-NEXT context until the FOR statement says we're done.

			//var loopVar = Convert.ToDouble(context.LoopVar.Evaluate(env));
			//var to = Convert.ToDouble(context.To.Evaluate(env));
			//var step = Convert.ToDouble(context.Step.Evaluate(env));

			//loopVar += step;
			//env.SetNumericVariableValue(LoopVar.Name, loopVar);

			//if (((step < 0) && (loopVar < to)) || ((step > 0) && (loopVar > to)))
			//{
			//	// We're done.
			//}
			//else
			//{
			//	// Goto loop start.
			//	env.CurrentLineNumber = context.BlockStartLineNumber;
			//	env.PushCallStack(context);  // We're still in the FOR-NEXT context.
			//}
		}

		public string ToListing()
		{
			return string.Concat("NEXT ", LoopVar.ToListing());
		}
	}
}
