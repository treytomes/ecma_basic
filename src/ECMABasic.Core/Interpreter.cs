using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using ECMABasic.Core.Parsers;
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
		private readonly List<StatementParser> _lineStatements;
		private readonly List<StatementParser> _immediateStatements;

		private ComplexTokenReader _reader;
		private readonly IBasicConfiguration _config;
		private readonly IEnvironment _env;

		public Interpreter(IEnvironment env, IBasicConfiguration config = null)
		{
			_env = env;
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
			};

			_immediateStatements = new List<StatementParser>()
			{
				new RunStatementParser(),
				new NewStatementParser(),
				new ContinueStatementParser(),
				new LoadStatementParser(),
				new ListStatementParser(),
			};
		}

		//public static Interpreter ImmediateMode(IErrorReporter reporter, IBasicConfiguration config = null)
		//{
		//	return new Interpreter(null, reporter, config, true);
		//}

		/// <summary>
		/// Create an interpreter that will interpret the input text directly.
		/// </summary>
		/// <param name="text">The text to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>The interpreter instance.</returns>
		public static void FromText(string text, IEnvironment env, IBasicConfiguration config = null)
		{
			var interpreter = new Interpreter(env, config);
			interpreter.InterpretProgramFromText(text);
		}

		/// <summary>
		/// Create an interpreter that will interpret the source text contained at the file path.
		/// </summary>
		/// <param name="path">The path to the file to interpret.</param>
		/// <param name="reporter">A receiver for error messages.</param>
		/// <returns>The interpreter instance.</returns>
		public static void FromFile(string path, IEnvironment env, IBasicConfiguration config = null)
		{
			var interpreter = new Interpreter(env, config);
			interpreter.InterpretProgramFromFile(path);
		}

		public void InterpretProgramFromFile(string path)
		{
			_reader = ComplexTokenReader.FromFile(path);
			InterpretProgram();
		}

		public void InterpretProgramFromText(string text)
		{
			_reader = ComplexTokenReader.FromText(text);
			InterpretProgram();
		}

		private void InterpretProgram()
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
						_env.Program.Insert(line);
					}
				}

				var token = _reader.Next();
				if (token != null)
				{
					throw new InvalidOperationException("Expected end-of-record.");
				}
			}
			catch (SyntaxException e)
			{
				_env.ReportError(e.Message);
				//_env.Program.Clear();
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

		// TODO: Some statements can only be run in immediate mode, some only in program mode, and some in both.  Need an indicator.

		public IStatement ProcessImmediate(string text)
		{
			try
			{
				_reader = ComplexTokenReader.FromText(text);

				var line = ProcessLine(false);
				if (line != null)
				{
					if (line.Statement != null)
					{
						_env.Program.Insert(line);
					}
					else
					{
						_env.Program.Delete(line.LineNumber);
					}
					return null;
				}
				else
				{
					ProcessSpace(false);
					var statement = ProcessStatement(null, false);
					if (statement == null)
					{
						statement = ProcessImmediateStatement();
					}

					ProcessSpace(false);
					ProcessEndOfLine();
					return statement;
				}
			}
			catch (SyntaxException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				throw new SyntaxException("SYNTAX ERROR");
			}
		}

		private ProgramLine ProcessLine(bool throwsOnError = true)
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

		private IStatement ProcessImmediateStatement()
		{
			foreach (var parser in _immediateStatements)
			{
				var stmt = parser.Parse(_reader);
				if (stmt != null)
				{
					return stmt;
				}
			}
			return null;
		}

		private IStatement ProcessStatement(int? lineNumber, bool throwOnError = true)
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
