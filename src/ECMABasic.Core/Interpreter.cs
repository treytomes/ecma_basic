using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	// TODO: The end-goal is to have a single interpreter that you can plug feature sets into, like adding BASIC-1 on top of Minimal BASIC.

	/// <summary>
	/// Convert the source text into an abstract syntax tree.
	/// </summary>
	public class Interpreter
	{
		private const int MAX_TAB_VALUE = 80; 
		
		private readonly ComplexTokenReader _reader;

		private Interpreter(ComplexTokenReader reader, IErrorReporter reporter)
		{
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
			const int MAX_LINE_NUMBER_DIGITS = 4;
			var lineNumber = _reader.NextInteger(MAX_LINE_NUMBER_DIGITS).Value;
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
			var ENDs = Program.Where(x => x.Statement.Type == StatementType.END);
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

		private Statement ProcessStatement(int lineNumber)
		{
			var token = _reader.Next();
			if (token.Type == TokenType.Keyword_END)
			{
				return ProcessStatementType(StatementType.END);
			}
			else if (token.Type == TokenType.Keyword_LET)
			{
				return ProcessStatementType(StatementType.LET);
			}
			else if (token.Type == TokenType.Keyword_PRINT)
			{
				return ProcessStatementType(StatementType.PRINT);
			}
			else if (token.Type == TokenType.Keyword_STOP)
			{
				return ProcessStatementType(StatementType.STOP);
			}	
			else
			{
				throw new LineSyntaxException("A STATEMENT WAS EXPECTED", lineNumber);
			}

			//        foreach (var statementType in Enum.GetValues<StatementType>())
			//        {
			//            var statement = ProcessStatementType(statementType, false);
			//            if (statement != null)
			//{
			//                return statement;
			//}
			//        }
			//        throw new InvalidOperationException("I expected to find a statement here.");
		}

		/// <summary>
		/// Process a series of tokens into the expected statement.
		/// </summary>
		/// <param name="type">The type of statement to process.</param>
		/// <param name="throwOnError">Should an exception be thrown if the statement type wasn't found?</param>
		/// <returns>The completed Statement object.</returns>
		private Statement ProcessStatementType(StatementType type, bool throwOnError = true)
		{
			if (type == StatementType.END)
			{
				return new EndStatement();
			}
			else if (type == StatementType.LET)
			{
				return ProcessLetStatement();
			}
			else if (type == StatementType.PRINT)
			{
				return ProcessPrintStatement();
			}
			else if (type == StatementType.STOP)
			{
				return new StopStatement();
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private Statement ProcessLetStatement()
		{
			ProcessSpace(true);

			var stringVariableToken = _reader.Next(TokenType.StringVariable);
			var targetExpr = new VariableExpression(stringVariableToken.Text);

			_reader.Next(TokenType.Equals);
			var valueToken = _reader.Next(TokenType.String, false);
			IExpression valueExpr;
			if (valueToken != null)
			{
				// The actual string is everything between the "".
				var text = valueToken.Text.Substring(1, valueToken.Text.Length - 2);
				valueExpr = new StringExpression(text);
			}
			else
			{
				valueToken = _reader.Next(TokenType.StringVariable);
				valueExpr = new VariableExpression(valueToken.Text);
			}

			return new LetStatement(targetExpr, valueExpr);
		}

		private Statement ProcessPrintStatement()
		{
			var spaceToken = ProcessSpace(false);
			if (spaceToken == null)
			{
				// It's just an empty print statement.
				return new PrintStatement();
			}

			var printList = ProcessPrintList();
			return new PrintStatement(printList);
		}

		private List<IExpression> ProcessPrintList()
		{
			var items = new List<IExpression>();

			while (true)
			{
				var printItem = ProcessPrintItem();
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

		private IExpression ProcessPrintItem()
		{
			var token = _reader.Next(TokenType.String, false);
			if (token != null)
			{
				// The actual string is everything between the "".
				var text = token.Text.Substring(1, token.Text.Length - 2);
				return new StringExpression(text);
			}

			token = _reader.Next(TokenType.StringVariable, false);
			if (token != null)
			{
				return new VariableExpression(token.Text);
			}
			
			token = _reader.Next(TokenType.Keyword_TAB, false);
			if (token != null)
			{
				_reader.Next(TokenType.OpenParenthesis);
				var tabValue = _reader.NextInteger().Value;
				_reader.Next(TokenType.CloseParenthesis);

				if (tabValue > MAX_TAB_VALUE)
				{
					throw new InvalidOperationException($"{tabValue} is too large for TAB.  Expected a value <= {MAX_TAB_VALUE}");
				}

				return new TabExpression(tabValue);
			}

			return null;
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

		private ProgramLine ProcessForBlock()
		{
			// TODO: Implement for-block processing.
			return null;
		}
	}
}
