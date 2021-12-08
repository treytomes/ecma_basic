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

		public string Evaluate(IEnvironment env)
		{
			var column = env.TerminalColumn / COLUMN_WIDTH;
			var nextColumnStart = (column + 1) * COLUMN_WIDTH;
			var numRemainingSpaces = nextColumnStart - env.TerminalColumn;
			var text = new string(' ', numRemainingSpaces);
			return text;
		}
	}
}
