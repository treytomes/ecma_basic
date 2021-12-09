using ECMABasic.Core.Exceptions;

namespace ECMABasic.Core.Statements
{
	public class StopStatement : Statement
	{
		public StopStatement()
			: base(StatementType.STOP)
		{
		}

		public override void Execute(IEnvironment env)
		{
			throw new ProgramEndException();
		}
	}
}
