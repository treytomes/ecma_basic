using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Expressions
{
	public class TabExpression : IExpression
	{
		public TabExpression(int value)
		{
			Value = value;
		}

		public int Value { get; }

		public string Evaluate(IEnvironment env)
		{
			var numSpaceNeeded = Value - env.TerminalColumn - 1;
			if (numSpaceNeeded < 0)
			{
				throw new RuntimeException("Cannot TAB backwards.");
			}

			var text = new string(' ', numSpaceNeeded);
			return text;
		}
	}
}
