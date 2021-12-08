using ECMABasic.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Test
{
	class TestEnvironment : IEnvironment
	{
		private readonly StringBuilder _sb = new StringBuilder();
		private readonly Dictionary<string, string> _stringVariables = new Dictionary<string, string>();

		/// <summary>
		/// The line number currently being executed.
		/// </summary>
		public int CurrentLineNumber { get; set; }

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

		public int TerminalRow { get; private set; } = 0;

		public int TerminalColumn { get; private set; } = 0;

		public void PrintLine(string text)
		{
			Print(text);
			_sb.AppendLine();
			TerminalRow++;
			TerminalColumn = 0;
		}

		public void Print(string text)
		{
			_sb.Append(text);
			TerminalColumn += text.Length;
		}

		public void ReportError(string message)
		{
			PrintLine(message);
		}

		public string GetStringVariableValue(string variableName)
		{
			if (!_stringVariables.ContainsKey(variableName))
			{
				var value = string.Empty;
				SetStringVariableValue(variableName, value);
				return value;
			}
			return _stringVariables[variableName];
		}

		public void SetStringVariableValue(string variableName, string value)
		{
			_stringVariables[variableName] = value;
		}
	}
}
