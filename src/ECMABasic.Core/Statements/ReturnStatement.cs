using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Statements
{
	public class ReturnStatement : IStatement
	{
		public ReturnStatement()
		{
		}

		public void Execute(IEnvironment env)
		{
			env.CurrentLineNumber = env.PopCallStack();
		}

		public string ToListing()
		{
			return "RETURN";
		}
	}
}
