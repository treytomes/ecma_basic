using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using System;
using System.Text;

namespace ECMABasic.Core.Expressions
{
	public class TabExpression : IPrintItem
	{
		private readonly IBasicConfiguration _config;

		public TabExpression(IExpression value, IBasicConfiguration config = null)
		{
			Value = value;
			_config = config ?? MinimalBasicConfiguration.Instance;
		}

		public IExpression Value { get; }

		public object Evaluate(IEnvironment env)
		{
			var value = (int)Math.Round(Convert.ToDouble(Value.Evaluate(env)));
			if (value < 1)
			{
				value = 1;

				// Report a non-fatal error, then continue execution.
				env.ReportError(new RuntimeException("TAB OUT OF RANGE", env.CurrentLineNumber).Message);
			}

			if (value > _config.TerminalWidth)
			{
				value -= _config.TerminalWidth * (int)((value - 1) / _config.TerminalWidth);
			}

			var sb = new StringBuilder();
			if (value < env.TerminalColumn)
			{
				sb.AppendLine();
				sb.Append(new string(' ', value));
			}
			else
			{
				var numSpaceNeeded = value - env.TerminalColumn - 1;
				sb.Append(new string(' ', numSpaceNeeded));
			}
			return sb.ToString();
		}

		public string ToListing()
		{
			return string.Concat("TAB(", Value.ToListing(), ")");
		}
	}
}
