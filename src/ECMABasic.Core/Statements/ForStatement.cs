using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;

namespace ECMABasic.Core.Statements
{
	public class ForStatement : IStatement
	{
		public ForStatement(VariableExpression loopVar, IExpression from, IExpression to, IExpression step)
		{
			if (!loopVar.IsNumeric)
			{
				throw new SyntaxException("EXPECTED A NUMERIC VARIABLE");
			}
			LoopVar = loopVar;
			From = from;
			To = to;
			Step = step;
		}

		public VariableExpression LoopVar { get; }
		public IExpression From { get; }
		public IExpression To { get; }
		public IExpression Step { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			var from = Convert.ToDouble(From.Evaluate(env));
			env.SetNumericVariableValue(LoopVar.Name, from);

			var returnToLineNumber = env.Program.GetNextLineNumber(env.CurrentLineNumber);
			env.PushCallStack(new ForStackContext(LoopVar, To, Step, returnToLineNumber));
		}

		public string ToListing()
		{
			return string.Concat("FOR ", LoopVar.ToListing(), "=", From.ToListing(), " TO ", To.ToListing(), " STEP ", Step.ToListing());
		}
	}
}
