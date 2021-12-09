using ECMABasic.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	/// <summary>
	/// A program is an executable list of program lines.
	/// </summary>
	public class Program : IEnumerable<ProgramLine>
	{
		private readonly Dictionary<int, ProgramLine> _lines;
		private readonly List<ProgramLine> _sortedLines;
		private readonly Dictionary<int, int> _lineNumberToIndex;

		public Program()
		{
			_lines = new Dictionary<int, ProgramLine>();
			_sortedLines = new List<ProgramLine>();
			_lineNumberToIndex = new Dictionary<int, int>();
		}

		public ProgramLine this[int lineNumber]
		{
			get
			{
				if (_lines.ContainsKey(lineNumber))
				{
					return _lines[lineNumber];
				}
				else
				{
					return null;
				}
			}
		}

		public int Length
		{
			get
			{
				return _sortedLines.Count;
			}
		}

		public void Execute(IEnvironment env)
		{
			if (Length == 0)
			{
				// Nothing to execute.
				return;
			}

			// Begin executing with the first line number.
			var lineIndex = 0;
			env.CurrentLineNumber = _sortedLines[lineIndex].LineNumber;

			while (true)
			{
				try
				{
					var oldLineNumber = env.CurrentLineNumber;
					var line = this[env.CurrentLineNumber];
					line.Statement.Execute(env);
					if (oldLineNumber == env.CurrentLineNumber)
					{
						// The statement didn't modify the current line number, so we can simply move to the next one.
						lineIndex++;
						env.CurrentLineNumber = _sortedLines[lineIndex].LineNumber;
					}
					else
					{
						// The statement modified the current line number, so we need to recalculate the line index.
						var nextLine = _lines[env.CurrentLineNumber];
						lineIndex = _lineNumberToIndex[env.CurrentLineNumber];
						// TODO: It would be worth creating an index of lineNumber-->lineIndex to speed things up a bit.
					}

					if (lineIndex >= Length)
					{
						throw new ProgramEndException();
					}
				}
				catch (ProgramEndException)
				{
					break;
				}
				catch (RuntimeException ex)
				{
					env.ReportError(ex.Message);
					break;
				}
			}
		}

		public void Insert(ProgramLine line)
		{
			_lines[line.LineNumber] = line;
			_sortedLines.Clear();
			_sortedLines.AddRange(_lines.OrderBy(x => x.Value.LineNumber).Select(x => x.Value));
			_lineNumberToIndex[line.LineNumber] = _sortedLines.IndexOf(line);
		}

		public void Clear()
		{
			_lines.Clear();
			_sortedLines.Clear();
		}

		public IEnumerator<ProgramLine> GetEnumerator()
		{
			foreach (var line in _sortedLines)
			{
				yield return line;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
