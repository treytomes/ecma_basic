using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Parsers;
using System;
using System.Collections.Generic;

namespace ECMABasic.Core
{
	// TODO: The end-goal is to have a single interpreter that you can plug feature sets into, like adding BASIC-1 on top of Minimal BASIC.

	/// <summary>
	/// Convert the source text into an abstract syntax tree.
	/// </summary>
	public class Interpreter
	{
		private readonly List<StatementParser> _lineStatements;

		protected ComplexTokenReader _reader;
		private readonly IBasicConfiguration _config;

		public Interpreter(IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;

			_lineStatements = new List<StatementParser>()
			{
				new EndStatementParser(),
				new LetStatementParser(),
				new PrintStatementParser(),
				new StopStatementParser(),
				new RemarkStatementParser(),
				new GotoStatementParser(),
				new GosubStatementParser(),
				new ReturnStatementParser(),
				new IfThenStatementParser(),
				new ForStatementParser(),
				new NextStatementParser(),
				new OnGotoStatementParser(),
				new RestoreStatementParser(),
				new ReadStatementParser(),
				new DataStatementParser(),
			};
		}

		/// <summary>
		/// Create an interpreter that will interpret the input text directly.
		/// </summary>
		/// <param name="text">The text to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>Was the input interpreted successfully?</returns>
		public static bool FromText(string text, IEnvironment env, IBasicConfiguration config = null)
		{
			var interpreter = new Interpreter(config);
			return interpreter.InterpretProgramFromText(env, text);
		}

		/// <summary>
		/// Create an interpreter that will interpret the source text contained at the file path.
		/// </summary>
		/// <param name="path">The path to the file to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>Was the input interpreted successfully?</returns>
		public static bool FromFile(string path, IEnvironment env, IBasicConfiguration config = null)
		{
			var interpreter = new Interpreter(config);
			return interpreter.InterpretProgramFromFile(env, path);
		}

		/// <summary>
		/// Allow interpretation of additional statements.
		/// </summary>
		/// <param name="additionalStatements">The statements to add to the interpreter.</param>
		public void InjectStatements(IEnumerable<StatementParser> additionalStatements)
		{
			_lineStatements.AddRange(additionalStatements);
		}

		/// <summary>
		/// Interpret the source text contained at the file path.
		/// </summary>
		/// <param name="path">The path to the file to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>Was the input interpreted successfully?</returns>
		public bool InterpretProgramFromFile(IEnvironment env, string path)
		{
			_reader = ComplexTokenReader.FromFile(path);
			return InterpretProgram(env);
		}

		/// <summary>
		/// Interpret the input text directly.
		/// </summary>
		/// <param name="text">The text to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>Was the input interpreted successfully?</returns>
		public bool InterpretProgramFromText(IEnvironment env, string text)
		{
			_reader = ComplexTokenReader.FromText(text);
			return InterpretProgram(env);
		}

		private bool InterpretProgram(IEnvironment env)
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
						env.Program.Insert(line);
					}
				}

				var token = _reader.Next();
				if (token != null)
				{
					throw new InvalidOperationException("Expected end-of-record.");
				}
				return true;
			}
			catch (SyntaxException e)
			{
				env.ReportError(e.Message);
				return false;
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

		protected ProgramLine ProcessLine(bool throwsOnError = true)
		{
			if (_reader.IsAtEnd)
			{
				return null;
			}

			var lineNumber = ProcessLineNumber(throwsOnError);
			if (!lineNumber.HasValue)
			{
				return null;
			}

			if (ProcessSpace(throwsOnError) == null)
			{
				// An empty line triggers a deletion.
				return new ProgramLine(lineNumber.Value, null);
			}

			var statement = ProcessStatement(lineNumber, throwsOnError);

			// Optional space.
			ProcessSpace(false);

			// Require an end-of-line.
			ProcessEndOfLine();

			return new ProgramLine(lineNumber.Value, statement);
		}

		/// <summary>
		/// Read a line number off of the token stream.
		/// An exception will occur if a line number could not be read.
		/// </summary>
		/// <returns>The line number.</returns>
		private int? ProcessLineNumber(bool throwsOnError = true)
		{
			var lineNumber = _reader.NextInteger(_config.MaxLineNumberDigits, throwsOnError);
			return lineNumber;
		}

		/// <summary>
		/// Read the end-of-line token off of the token stream.
		/// An exception will occur if the end-of-line could not be read.
		/// </summary>
		protected void ProcessEndOfLine()
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
		protected Token ProcessSpace(bool throwOnError = true)
		{
			return _reader.Next(TokenType.Space, throwOnError);
		}

		protected IStatement ProcessStatement(int? lineNumber, bool throwOnError = true)
		{
			foreach (var parser in _lineStatements)
			{
				var stmt = parser.Parse(_reader, lineNumber);
				if (stmt != null)
				{
					return stmt;
				}
			}

			if (throwOnError)
			{
				throw new SyntaxException("A STATEMENT WAS EXPECTED", lineNumber);
			}
			return null;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
		private ProgramLine ProcessForBlock()
		{
			// TODO: Implement for-block processing.
			return null;
		}
	}
}
