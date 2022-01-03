namespace ECMABasic.Core.Statements
{
	public class RestoreStatement : IStatement
	{
		public void Execute(IEnvironment env, bool isImmediate)
		{
			env.ResetDataPointer();
		}

		public string ToListing()
		{
			return "RESTORE";
		}
	}
}
