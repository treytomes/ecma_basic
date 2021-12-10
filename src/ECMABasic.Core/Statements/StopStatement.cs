using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class StopStatement : IStatement
	{
		public void Execute(IEnvironment env)
		{
			throw new ProgramEndException();
		}
	}
}
