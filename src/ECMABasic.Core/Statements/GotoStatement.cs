using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core.Statements
{
	public class GotoStatement : IStatement
	{
		public GotoStatement(IExpression lineNumber)
		{
			LineNumber = lineNumber;
		}

		public IExpression LineNumber { get; }

		public virtual void Execute(IEnvironment env)
		{
			var lineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
			if (!env.ValidateLineNumber(lineNumber, false))
			{
				throw new LineRuntimeException($"UNDEFINED LINE NUMBER {lineNumber}", env.CurrentLineNumber);
			}
			env.CurrentLineNumber = lineNumber;
		}

		public virtual string ToListing()
		{
			return string.Concat("GOTO ", LineNumber.ToListing());
		}
	}
}
