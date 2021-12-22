using ECMABasic.Core;
using ECMABasic.Core.Statements;

namespace ECMABasic55.Statements
{
	/// <summary>
	/// Continue after a program has been stopped.
	/// </summary>
	public class ContinueStatement : IStatement
	{
		// TODO: ?CN ERROR if there if the program wasn't STOPped.

		public void Execute(IEnvironment env)
		{
			if (env.Program[env.CurrentLineNumber].Statement is StopStatement)
			{
				env.Program.MoveToNextLine(env);
			}
			env.Program.Execute(env);
		}

		public string ToListing()
		{
			return "CONT";
		}
	}
}
