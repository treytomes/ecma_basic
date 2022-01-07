using System;

namespace ECMABasic.Core.Statements
{
	// TODO: # GOTO # needs to start an infinite loop.
	public class GotoStatement : IStatement
	{
		public GotoStatement(IExpression lineNumber)
		{
			if (lineNumber.Type != ExpressionType.Number)
			{
				throw ExceptionFactory.ExpectedNumericExpression();
			}
			LineNumber = lineNumber;
		}

		public IExpression LineNumber { get; }

		public virtual void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}

			var lineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
			if (!env.ValidateLineNumber(lineNumber, false))
			{
				throw ExceptionFactory.UndefinedLineNumber(lineNumber, env.CurrentLineNumber);
			}
			env.CurrentLineNumber = lineNumber;

			var context = env.PopCallStack();
			if (context is ForStackContext)
			{
				var forContext = context as ForStackContext;
				if ((env.CurrentLineNumber < forContext.BlockStartLineNumber) || (env.CurrentLineNumber >= forContext.BlockEndLineNumber))
				{
					// We just jumped out of the FOR-block, so we can dispose of the context.
				}
				else
				{
					env.PushCallStack(context);
				}
			}
			else
			{
				if (context != null)
				{
					env.PushCallStack(context);
				}
			}
		}

		public virtual string ToListing()
		{
			return string.Concat("GOTO ", LineNumber.ToListing());
		}
	}
}
