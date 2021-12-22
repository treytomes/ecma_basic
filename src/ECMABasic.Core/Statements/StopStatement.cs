using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class StopStatement : IStatement
	{
		public void Execute(IEnvironment env, bool isImmediate)
		{
			throw new ProgramStopException(env.CurrentLineNumber);
		}

		// TODO: Centralize keyword strings to make them easier to change?

		public string ToListing()
		{
			return "STOP";
		}
	}
}
