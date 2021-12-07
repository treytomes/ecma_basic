using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	public class EndStatement : Statement
	{
		public EndStatement()
			: base(StatementType.END)
		{
		}

		public override void Execute(IEnvironment env)
		{
			throw new ProgramEndException();
		}
	}
}
