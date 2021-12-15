namespace ECMABasic.Core.Statements
{
	/// <summary>
	/// Continue after a program has been stopped.
	/// </summary>
	public class ContinueStatement : IStatement
	{
		public void Execute(IEnvironment env)
		{
			if (env.Program[env.CurrentLineNumber].Statement is StopStatement)
			{
				env.Program.MoveToNextLine(env);
			}
			env.Program.Execute(env);
		}
	}
}
