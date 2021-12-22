namespace ECMABasic.Core.Statements
{
	public class ReturnStatement : IStatement
	{
		public ReturnStatement()
		{
		}

		// TODO: Need a "% RETURN WITHOUT GOSUB".

		public void Execute(IEnvironment env)
		{
			env.CurrentLineNumber = env.PopCallStack();
		}

		public string ToListing()
		{
			return "RETURN";
		}
	}
}
