using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	/// <summary>
	/// A program is an executable list of program lines.
	/// </summary>
	public class Program : IEnumerable<ProgramLine>, IListable
	{
		private readonly Dictionary<int, ProgramLine> _lines = new();
		private readonly List<ProgramLine> _sortedLines = new();
		private readonly Dictionary<int, int> _lineNumberToIndex = new();

		/// <summary>
		/// Maintain a list of line indices that contain DATA statements.
		/// </summary>
		private readonly List<int> _datas = new();

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

			try
			{
				ValidateEndLine();

				// Begin executing with the first line number.
				var lineIndex = 0;
				if (env.CurrentLineNumber == 0)
				{
					env.CurrentLineNumber = _sortedLines[lineIndex].LineNumber;
				}
				else
				{
					lineIndex = _lineNumberToIndex[env.CurrentLineNumber];
				}

				while (true)
				{
					env.CheckForStopRequest();

					var oldLineNumber = env.CurrentLineNumber;
					var line = this[env.CurrentLineNumber];
					line.Statement.Execute(env, false);
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
					}

					if (lineIndex >= Length)
					{
						throw ExceptionFactory.ProgramEnd(env.CurrentLineNumber);
					}
				}
			}
			catch (ProgramEndException)
			{
			}
			catch (RuntimeException ex)
			{
				env.ReportError(ex.Message);
			}
			catch (SyntaxException ex)
			{
				env.ReportError(ex.Message);
			}
		}

		public int GetNextLineNumber(int fromLineNumber)
		{
			var lineIndex = _lineNumberToIndex[fromLineNumber];
			lineIndex++;
			if (lineIndex < Length)
			{
				return _sortedLines[lineIndex].LineNumber;
			}
			else
			{
				return -1;
			}
		}

		public ProgramLine MoveToNextLine(IEnvironment env)
		{
			env.CurrentLineNumber = GetNextLineNumber(env.CurrentLineNumber);
			return this[env.CurrentLineNumber];
		}

		public void Insert(ProgramLine line)
		{
			_lines[line.LineNumber] = line;
			RebuildIndex();
		}
		
		public void Delete(int lineNumber)
		{
			if (_lines.ContainsKey(lineNumber))
			{
				_lines.Remove(lineNumber);
				RebuildIndex();
			}
		}

		/// <summary>
		/// Retrieve a data line.
		/// </summary>
		/// <param name="n">The data index (which is not the line number).</param>
		/// <returns>The DATA statement, or null if out of range.</returns>
		/// <exception cref="RuntimeException">Thrown if the line number doesn't contain a DATA statement.</exception>
		public DataStatement GetDataLine(int n)
		{
			if (n >= _datas.Count)
			{
				return null;
			}

			var line = _sortedLines[_datas[n]];
			var stmt = line.Statement;
			if (stmt is DataStatement)
			{
				return stmt as DataStatement;
			}
			else
			{
				throw ExceptionFactory.ExpectedData(line.LineNumber);
			}
		}

		private void RebuildIndex()
		{
			_sortedLines.Clear();
			_lineNumberToIndex.Clear();
			_datas.Clear();

			var n = 0;
			foreach (var l in _lines.OrderBy(x => x.Value.LineNumber).Select(x => x.Value))
			{
				_sortedLines.Add(l);
				_lineNumberToIndex[l.LineNumber] = n;
				if (l.Statement is DataStatement)
				{
					_datas.Add(n);
				}
				n++;
			}
		}

		public void Clear()
		{
			_lines.Clear();
			_sortedLines.Clear();
		}

		public string ToListing()
		{
			return string.Join(string.Empty, _lines.Select(x => x.Value.ToListing()));
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

		private void ValidateEndLine()
		{
			var ENDs = this.Where(x => x.Statement is EndStatement);
			if (!ENDs.Any())
			{
				throw ExceptionFactory.NoEndInstruction();
			}

			var lastLineNumber = this.Max(x => x.LineNumber);
			if (ENDs.First().LineNumber != lastLineNumber)
			{
				throw ExceptionFactory.EndIsNotLast(ENDs.First().LineNumber);
			}
		}
	}
}
