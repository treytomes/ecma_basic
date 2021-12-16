using ECMABasic.Core.Exceptions;
using System;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	/// <summary>
	/// Run the current program, optionally specifying a line number to start at.
	/// </summary>
	public class RunStatement : IStatement
	{
		public RunStatement(IExpression lineNumber)
		{
			LineNumber = lineNumber;
		}

		public IExpression LineNumber { get; }

		public void Execute(IEnvironment env)
		{
			if (LineNumber != null)
			{
				var lineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
				if (!env.Program.Any(x => x.LineNumber == lineNumber))
				{
					throw new RuntimeException($"% LINE NUMBER {lineNumber} IS NOT DEFINED");
				}
				env.CurrentLineNumber = lineNumber;
			}
			else
			{
				if (env.Program.Length > 0)
				{
					env.CurrentLineNumber = env.Program.OrderBy(x => x.LineNumber).First().LineNumber;
				}
			}
			env.Program.Execute(env);
		}
	}
}
