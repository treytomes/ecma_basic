using ECMABasic.Core.Configuration;

namespace ECMABasic.Core.Expressions
{
	/// <summary>
	/// Used when a comma occurs in a print list.
	/// </summary>
	public class CommaExpression : IPrintItemSeparator
	{
		private readonly IBasicConfiguration _config;

		public CommaExpression(IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
		}

		public object Evaluate(IEnvironment env)
		{
			var column = env.TerminalColumn / _config.TerminalColumnWidth;
			var nextColumn = column + 1;
			if (nextColumn >= _config.NumTerminalColumns)
			{
				return "\n";
			}
			var nextColumnStart = nextColumn * _config.TerminalColumnWidth;
			var numRemainingSpaces = nextColumnStart - env.TerminalColumn;
			var text = new string(' ', numRemainingSpaces);
			return text;
		}

		public string ToListing()
		{
			return ",";
		}
	}
}
