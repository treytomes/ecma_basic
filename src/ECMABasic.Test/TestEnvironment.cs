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

		public void PrintLine(string text)
		{
			_sb.AppendLine(text);
		}
	}
}
