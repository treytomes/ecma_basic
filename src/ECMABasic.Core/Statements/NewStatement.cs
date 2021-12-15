namespace ECMABasic.Core.Statements
{
	public class NewStatement : IStatement
	{
		public void Execute(IEnvironment env)
		{
			env.Clear();
		}
	}
}
