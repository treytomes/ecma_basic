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
				env.CurrentLineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
			}
			else
			{
				env.CurrentLineNumber = env.Program.OrderBy(x => x.LineNumber).First().LineNumber;
			}
			env.Program.Execute(env);
		}
	}
}
