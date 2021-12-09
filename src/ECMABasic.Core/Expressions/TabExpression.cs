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
		private const int MAX_TAB_VALUE = 80;

		public TabExpression(IExpression value)
		{
			Value = value;
		}

		public IExpression Value { get; }

		public object Evaluate(IEnvironment env)
		{
			var value = (int)Math.Round(Convert.ToDouble(Value.Evaluate(env)));
			if (value < 1)
			{
				value = 1;

				// Report a non-fatal error, then continue execution.
				env.ReportError(new LineRuntimeException("TAB OUT OF RANGE", env.CurrentLineNumber).Message);
			}

			if (value > MAX_TAB_VALUE)
			{
				value = value - MAX_TAB_VALUE * (int)((value - 1) / MAX_TAB_VALUE);
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
	}
}
