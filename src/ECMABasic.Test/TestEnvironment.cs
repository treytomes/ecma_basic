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
		private readonly StringBuilder _sb = new();

		public override int TerminalRow { get; set; } = 0;

		public override int TerminalColumn { get; set; } = 0;

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
			Debug.WriteLine(string.Empty);
			TerminalRow++;
			TerminalColumn = 0;
		}

		public override void Print(string text)
		{
			_sb.Append(text);
			Debug.Write(text);
			TerminalColumn += text.Length;
		}

		public override void ReportError(string message)
		{
			if (TerminalColumn != 0)
			{
				PrintLine(string.Empty);
			}
			PrintLine(message);
		}

		public override void CheckForStopRequest()
		{
			// There's no stopping the test environment.
		}
	}
}
