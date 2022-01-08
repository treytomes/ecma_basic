using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;

namespace ECMABasic.Core.Statements
{
	public class ForStatement : IStatement
	{
		public ForStatement(VariableExpression loopVar, IExpression from, IExpression to, IExpression step)
		{
			if (!loopVar.IsNumeric)
			{
				throw ExceptionFactory.ExpectedNumericVariable();
			}
			LoopVar = loopVar;
			From = from;
			To = to;
			Step = step;
		}

		public VariableExpression LoopVar { get; }
		public IExpression From { get; }
		public IExpression To { get;}
		public IExpression Step { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}

			var context = env.PopCallStack();

			if ((context is not ForStackContext forContext) || (forContext?.LoopVar?.Name != LoopVar.Name))
			{
				// Either there's no FOR-context, or it's the wrong FOR-context.
				if (context != null)
				{
					// It's not ours, so put it back.
					env.PushCallStack(context);
				}

				// Null it out so we know to make a new one.
				forContext = null;
			}

			if (forContext == null)
			{
				// This is the first time, so we need to build the context.
				var from = Convert.ToDouble(From.Evaluate(env));
				var to = Convert.ToDouble(To.Evaluate(env));
				var step = Convert.ToDouble(Step.Evaluate(env));
				env.SetNumericVariableValue(LoopVar.Name, from);

				// Come back here when the loop is done (NEXT) to reevaluate the exit parameters.
				var returnToLineNumber = env.CurrentLineNumber; // env.Program.GetNextLineNumber(env.CurrentLineNumber);
				forContext = new ForStackContext(LoopVar, to, step, returnToLineNumber);
			}
			else
			{
				if (forContext?.LoopVar?.Name == LoopVar.Name)
				{
					// After the first time through the loop, we increment the control variable.
					env.SetNumericVariableValue(LoopVar.Name, Convert.ToDouble(LoopVar.Evaluate(env)) + forContext.Step);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			// We've already been through this loop once.
			var own1 = forContext.To;
			var own2 = forContext.Step;
			var v = Convert.ToDouble(LoopVar.Evaluate(env));

			if ((v - own1) * Math.Sign(own2) > 0)
			{
				// Leave the for-block.

				// Either get the block end line number, or search for it.
				if (forContext.BlockEndLineNumber.HasValue)
				{
					env.CurrentLineNumber = forContext.BlockEndLineNumber.Value;
				}
				else
				{
					// Scan for NEXT.
					while (!(env.Program.MoveToNextLine(env)?.Statement is NextStatement)) ;
					// It'll stop incrementing when the NEXT is found.

					// Move to the line *just after* the NEXT to continue running.
					env.Program.MoveToNextLine(env);
				}
			}
			else
			{
				// We should be able to just enter the loop.

				// Push the call stack so we're ready to do this again.
				env.PushCallStack(forContext);
			}
		}

		public string ToListing()
		{
			return string.Concat("FOR ", LoopVar.ToListing(), "=", From.ToListing(), " TO ", To.ToListing(), " STEP ", Step.ToListing());
		}
	}
}
