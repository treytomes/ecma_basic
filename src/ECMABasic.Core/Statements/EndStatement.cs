using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class EndStatement : IStatement
	{
		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}
			throw ExceptionFactory.ProgramEnd(env.CurrentLineNumber);
		}

		public string ToListing()
		{
			return "END";
		}
	}
}
