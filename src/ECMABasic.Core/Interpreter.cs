using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	// TODO: The end-goal is to have a single interpreter that you can plug feature sets into, like adding BASIC-1 on top of Minimal BASIC.

	/// <summary>
	/// Convert the source text into an abstract syntax tree.
	/// </summary>
	public class Interpreter
	{
		private readonly ComplexTokenReader _reader;
		private readonly IBasicConfiguration _config;

		private Interpreter(ComplexTokenReader reader, IErrorReporter reporter, IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
			_reader = reader;
			Program = new Program();
			ProcessProgram(reporter);
		}

		/// <summary>
		/// The full program, ready to execute.
		/// </summary>
		public Program Program { get; }

		/// <summary>
		/// Create an interpreter that will interpret the input text directly.
		/// </summary>
		/// <param name="text">The text to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>The interpreter instance.</returns>
		public static Interpreter FromText(string text, IErrorReporter reporter)
		{
			return new Interpreter(ComplexTokenReader.FromText(text), reporter);
		}

		/// <summary>
		/// Create an interpreter that will interpret the source text contained at the file path.
		/// </summary>
		/// <param name="path">The path to the file to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>The interpreter instance.</returns>
		public static Interpreter FromFile(string path, IErrorReporter reporter)
		{
			return new Interpreter(ComplexTokenReader.FromFile(path), reporter);
		}

		private void ProcessProgram(IErrorReporter reporter)
		{
			try
			{
				while (true)
				{
					var line = ProcessBlock();
					if (line == null)
					{
						break;
					}
					else
					{
						Program.Insert(line);
					}
				}

				ValidateEndLine();

				var token = _reader.Next();
				if (token != null)
				{
					throw new InvalidOperationException("Expected end-of-record.");
				}
			}
			catch (SyntaxException e)
			{
				reporter.ReportError(e.Message);
				Program.Clear();
			}
		}

		/// <summary>
		/// Process the next line or for-block.
		/// </summary>
		/// <returns></returns>
		private ProgramLine ProcessBlock()
		{
			var line = ProcessLine();
			if (line != null)
			{
				return line;
			}

			line = ProcessForBlock();
			if (line != null)
			{
				return line;
			}
			else
			{
				return null;
			}
		}

		private ProgramLine ProcessLine()
		{
			if (_reader.IsAtEnd)
			{
				return null;
			}

			var lineNumber = ProcessLineNumber();

			ProcessSpace();

			var statement = ProcessStatement(lineNumber);

			// Optional space.
			ProcessSpace(false);

			// Require an end-of-line.
			ProcessEndOfLine();

			return new ProgramLine(lineNumber, statement);
		}

		/// <summary>
		/// Read a line number off of the token stream.
		/// An exception will occur if a line number could not be read.
		/// </summary>
		/// <returns>The line number.</returns>
		private int ProcessLineNumber()
		{
			var lineNumber = _reader.NextInteger(_config.MaxLineNumberDigits).Value;
			return lineNumber;
		}

		/// <summary>
		/// Read the end-of-line token off of the token stream.
		/// An exception will occur if the end-of-line could not be read.
		/// </summary>
		private void ProcessEndOfLine()
		{
			var next = _reader.Next();
			if (next == null)
			{
				return;
			}
			if (next.Type != TokenType.EndOfLine)
			{
				throw new UnexpectedTokenException(TokenType.EndOfLine, next);
			}
		}

		/// <summary>
		/// Read whitespace off of the token stream.
		/// An exception will optionally occur if whitespace could not be found.
		/// </summary>
		/// <param name="throwOnError">Throw an exception if space is not found.  Default to true.</param>
		/// <returns>The space token.</returns>
		private Token ProcessSpace(bool throwOnError = true)
		{
			return _reader.Next(TokenType.Space, throwOnError);
		}

		private void ValidateEndLine()
		{
			var ENDs = Program.Where(x => x.Statement is EndStatement);
			if (!ENDs.Any())
			{
				throw new NoEndInstructionException();
			}

			var lastLineNumber = Program.Max(x => x.LineNumber);
			if (ENDs.First().LineNumber != lastLineNumber)
			{
				throw new SyntaxException("END IS NOT LAST", ENDs.First().LineNumber);
			}
		}

		private IStatement ProcessStatement(int lineNumber)
		{
			IStatement stmt;

			stmt = new EndStatementParser().Parse(_reader, lineNumber);
			if (stmt != null)
			{
				return stmt;
			}

			stmt = new LetStatementParser().Parse(_reader, lineNumber);
			if (stmt != null)
			{
				return stmt;
			}

			stmt = new PrintStatementParser().Parse(_reader, lineNumber);
			if (stmt != null)
			{
				return stmt;
			}

			stmt = new StopStatementParser().Parse(_reader, lineNumber);
			if (stmt != null)
			{
				return stmt;
			}

			throw new SyntaxException("A STATEMENT WAS EXPECTED", lineNumber);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
		private ProgramLine ProcessForBlock()
		{
			// TODO: Implement for-block processing.
			return null;
		}
	}
}
