using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core.Statements
{
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
		}

		public virtual string ToListing()
		{
			return string.Concat("GOTO ", LineNumber.ToListing());
		}
	}
}
