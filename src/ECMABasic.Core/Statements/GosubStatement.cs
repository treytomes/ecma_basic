using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core.Statements
{
	class GosubStatement : IStatement
	{
		public GosubStatement(IExpression lineNumber)
		{
			LineNumber = lineNumber;
		}

		public IExpression LineNumber { get; }

		public void Execute(IEnvironment env)
		{
			var lineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
			if (!env.ValidateLineNumber(lineNumber, false))
			{
				throw new LineRuntimeException($"UNDEFINED LINE NUMBER {lineNumber}", env.CurrentLineNumber);
			}

			var returnToLineNumber = env.Program.GetNextLineNumber(env.CurrentLineNumber);
			env.PushCallStack(returnToLineNumber);
			env.CurrentLineNumber = lineNumber;
		}

		public string ToListing()
		{
			return string.Concat("GOSUB ", LineNumber.ToListing());
		}
	}
}
