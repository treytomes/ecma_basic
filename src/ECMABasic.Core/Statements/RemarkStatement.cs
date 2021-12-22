namespace ECMABasic.Core.Statements
{
	public class RemarkStatement : IStatement
	{
		public RemarkStatement(string remark)
		{
			Remark = remark;
		}

		public string Remark { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			// This statement doesn't actually affect the environment at all.
		}

		public string ToListing()
		{
			return string.Concat("REM ", Remark);
		}
	}
}
