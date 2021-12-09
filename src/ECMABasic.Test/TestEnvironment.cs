using ECMABasic.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Test
{
	class TestEnvironment : EnvironmentBase
	{
		private readonly StringBuilder _sb = new StringBuilder();

		public string Text
		{
			get
			{
				return _sb.ToString();
			}
		}

		public string[] Lines
		{
			get
			{
				return Text.Split('\n');
			}
		}

		public override void PrintLine(string text)
		{
			Print(text);
			_sb.AppendLine();
			TerminalRow++;
			TerminalColumn = 0;
		}

		public override void Print(string text)
		{
			_sb.Append(text);
			TerminalColumn += text.Length;
		}

		public override void ReportError(string message)
		{
			PrintLine(message);
		}
	}
}
