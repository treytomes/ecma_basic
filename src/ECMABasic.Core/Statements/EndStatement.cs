using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class EndStatement : IStatement
	{
		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw new SyntaxException("ONLY ALLOWED IN PROGRAM");
			}
			throw new ProgramEndException();
		}

		public string ToListing()
		{
			return "END";
		}
	}
}
