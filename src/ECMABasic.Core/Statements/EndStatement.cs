using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class EndStatement : IStatement
	{
		public void Execute(IEnvironment env)
		{
			throw new ProgramEndException();
		}
	}
}
