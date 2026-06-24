

namespace ECMABasic.Domain.Expressions;

/// <summary>
/// Used when a comma occurs in a print list.
/// </summary>
public class CommaExpression : IPrintItemSeparator
{
	public CommaExpression()
	{
	}

	public object Evaluate(IEnvironment env)
	{
		var config = env.Configuration;
		var column = env.TerminalColumn / config.TerminalColumnWidth;
		var nextColumn = column + 1;
		if (nextColumn >= config.NumTerminalColumns)
		{
			return "\n";
		}
		var nextColumnStart = nextColumn * config.TerminalColumnWidth;
		var numRemainingSpaces = nextColumnStart - env.TerminalColumn;
		var text = new string(' ', numRemainingSpaces);
		return text;
	}

	public string ToListing()
	{
		return ",";
	}
}
