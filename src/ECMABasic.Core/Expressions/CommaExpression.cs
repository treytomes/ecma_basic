using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Expressions
{
	/// <summary>
	/// Used when a comma occurs in a print list.
	/// </summary>
	public class CommaExpression : IExpression
	{
		private const int TERMINAL_WIDTH = 80;  // TODO: This should be a global setting, shared with the TAB checker.
		private const int NUM_COLUMNS = 5;
		private const int COLUMN_WIDTH = TERMINAL_WIDTH / NUM_COLUMNS;

		public object Evaluate(IEnvironment env)
		{
			var column = env.TerminalColumn / COLUMN_WIDTH;
			var nextColumn = column + 1;
			if (nextColumn >= NUM_COLUMNS)
			{
				return "\n";
			}
			var nextColumnStart = nextColumn * COLUMN_WIDTH;
			var numRemainingSpaces = nextColumnStart - env.TerminalColumn;
			var text = new string(' ', numRemainingSpaces);
			return text;
		}
	}
}
