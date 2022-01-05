using ECMABasic.Core;
using ECMABasic.Core.Exceptions;

namespace ECMABasic55.Statements
{
	public class NewStatement : IStatement
	{
		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (!isImmediate)
			{
				throw ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
			}

			env.Clear();
		}

		public string ToListing()
		{
			return "NEW";
		}
	}
}
