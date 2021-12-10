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
				throw new LineSyntaxException("END IS NOT LAST", ENDs.First().LineNumber);
			}
		}

		private IStatement ProcessStatement(int lineNumber)
		{
			IStatement stmt;

			stmt = ProcessEndStatement();
			if (stmt != null)
			{
				return stmt;
			}

			stmt = ProcessLetStatement();
			if (stmt != null)
			{
				return stmt;
			}

			stmt = ProcessPrintStatement(lineNumber);
			if (stmt != null)
			{
				return stmt;
			}

			stmt = ProcessStopStatement();
			if (stmt != null)
			{
				return stmt;
			}

			throw new LineSyntaxException("A STATEMENT WAS EXPECTED", lineNumber);
		}

		private IStatement ProcessEndStatement()
		{
			var token = _reader.Next(TokenType.Word, false, "END");
			if (token == null)
			{
				return null;
			}
			return new EndStatement();
		}

		private IStatement ProcessLetStatement()
		{
			var token = _reader.Next(TokenType.Word, false, "LET");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(true);

			VariableExpression targetExpr;
			var variableNameToken = _reader.Next(TokenType.StringVariable, false);
			if (variableNameToken != null)
			{
				targetExpr = new VariableExpression(variableNameToken.Text);
			}
			else
			{
				variableNameToken = _reader.Next(TokenType.NumericVariable);
				targetExpr = new VariableExpression(variableNameToken.Text);
			}

			ProcessSpace(false);

			_reader.Next(TokenType.Equals);

			ProcessSpace(false);

			Token valueToken;
			IExpression valueExpr;
			if (targetExpr.IsString)
			{
				valueToken = _reader.Next(TokenType.String, false);
				if (valueToken != null)
				{
					// The actual string is everything between the "".
					var text = valueToken.Text[1..^1];
					valueExpr = new StringExpression(text);
				}
				else
				{
					valueToken = _reader.Next(TokenType.StringVariable);
					valueExpr = new VariableExpression(valueToken.Text);
				}
			}
			else
			{
				var isNegative = _reader.Next(TokenType.Negation, false) != null;

				var number = _reader.NextNumber(false);
				if (number != null)
				{
					if (isNegative)
					{
						number = -number.Value;
					}
					valueExpr = new NumberExpression(number.Value);
				}
				else
				{
					valueToken = _reader.Next(TokenType.NumericVariable);
					valueExpr = new VariableExpression(valueToken.Text);
				}
			}

			return new LetStatement(targetExpr, valueExpr);
		}

		private IStatement ProcessPrintStatement(int lineNumber)
		{
			var token = _reader.Next(TokenType.Word, false, "PRINT");
			if (token == null)
			{
				return null;
			}

			var spaceToken = ProcessSpace(false);
			if (spaceToken == null)
			{
				// It's just an empty print statement.
				return new PrintStatement();
			}

			var printList = ProcessPrintList(lineNumber);
			return new PrintStatement(printList);
		}

		private List<IPrintItem> ProcessPrintList(int lineNumber)
		{
			var items = new List<IPrintItem>();

			while (true)
			{
				var printItem = ProcessPrintItem(lineNumber);
				if (printItem != null)
				{
					items.Add(printItem);
				}

				ProcessSpace(false);  // Optional space.

				var printSeparator = ProcessPrintSeparator();

				ProcessSpace(false);  // Optional space.

				if (printSeparator == null)
				{
					// No separator, so this is the end of the list.
					return items;
				}
				else
				{
					// If there's a separator, there might be another item.
					items.Add(printSeparator);
				}
			}
		}

		private IPrintItem ProcessPrintItem(int lineNumber)
		{
			var token = _reader.Next(TokenType.String, false);
			if (token != null)
			{
				// The actual string is everything between the "".
				var text = token.Text[1..^1];
				return new StringExpression(text);
			}

			token = _reader.Next(TokenType.StringVariable, false);
			if (token != null)
			{
				return new VariableExpression(token.Text);
			}

			var expr = ProcessTabExpression(lineNumber);
			if (expr != null)
			{
				return expr;
			}

			return null;
		}

		private IPrintItem ProcessTabExpression(int lineNumber)
		{
			var token = _reader.Next(TokenType.Word, false, "TAB");
			if (token == null)
			{
				return null;
			}

			_reader.Next(TokenType.OpenParenthesis);

			IExpression valueExpr = ProcessVariableExpression();
			if (valueExpr == null)
			{
				valueExpr = ProcessNumberExpression();
				if (valueExpr == null)
				{
					throw new LineSyntaxException("A TAB ARGUMENT WAS EXPECTED", lineNumber);
				}
			}
			_reader.Next(TokenType.CloseParenthesis);

			return new TabExpression(valueExpr);
		}

		private IExpression ProcessVariableExpression()
		{
			var valueToken = _reader.Next(TokenType.NumericVariable, false);
			if (valueToken == null)
			{
				return null;
			}
			return new VariableExpression(valueToken.Text);
		}

		private IExpression ProcessNumberExpression()
		{
			var tabValue = _reader.NextNumber(false);
			if (!tabValue.HasValue)
			{
				return null;
			}
			return new NumberExpression(tabValue.Value);
		}

		private IExpression ProcessPrintSeparator()
		{
			var symbolToken = _reader.Next(TokenType.Comma, false);
			if (symbolToken != null)
			{
				return new CommaExpression();
			}

			symbolToken = _reader.Next(TokenType.Semicolon, false);
			if (symbolToken != null)
			{
				return new SemicolonExpression();
			}

			return null;
		}

		private IStatement ProcessStopStatement()
		{
			var token = _reader.Next(TokenType.Word, false, "STOP");
			if (token == null)
			{
				return null;
			}
			return new StopStatement();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
		private ProgramLine ProcessForBlock()
		{
			// TODO: Implement for-block processing.
			return null;
		}
	}
}
