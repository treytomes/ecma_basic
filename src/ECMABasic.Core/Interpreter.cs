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
        private readonly ComplexTokenReader _reader;

        private Interpreter(ComplexTokenReader reader)
		{
            _reader = reader;
            Program = new Program();
            ProcessProgram();
		}

        /// <summary>
        /// The full program, ready to execute.
        /// </summary>
        public Program Program { get; }

        public static Interpreter FromText(string text)
		{
            return new Interpreter(ComplexTokenReader.FromText(text));
		}

        public static Interpreter FromFile(string file)
		{
            return new Interpreter(ComplexTokenReader.FromFile(file));
		}

        private void ProcessProgram()
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

            if (Program.Last().Statement.Type != StatementType.END)
			{
                throw new InvalidOperationException("The program must end with an END.");
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

            var statement = ProcessStatement();

            ProcessSpace(false);

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
        private void ProcessSpace(bool throwOnError = true)
		{
            _reader.Next(TokenType.Space, throwOnError);
		}

        private ProgramLine ProcessEndLine()
        {
            var lineNumber = ProcessLineNumber();

            ProcessSpace();

            var statement = ProcessStatementType(StatementType.END);

            ProcessSpace(false);

            ProcessEndOfLine();

            return new ProgramLine(lineNumber, statement);
        }

        private Statement ProcessStatement()
        {
            var token = _reader.Next();
            if (token.Type == TokenType.Keyword_END)
			{
                return ProcessStatementType(StatementType.END);
			}
            else if (token.Type == TokenType.Keyword_PRINT)
			{
                return ProcessStatementType(StatementType.PRINT);
			}
            else
			{
                throw new InvalidOperationException("I expected to find a statement here.");
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
            else if (type == StatementType.PRINT)
			{
                var spaceToken = _reader.Next(TokenType.Space, false);
                if (spaceToken == null)
				{
                    return new PrintStatement(new StringExpression(string.Empty));
                    //throw new UnexpectedTokenException(TokenType.Space, spaceToken);
				}

                var stringToken = _reader.Next(TokenType.String, false);
                // The actual string is everything between the "".
                var text = (stringToken == null) ? string.Empty : stringToken.Text.Substring(1, stringToken.Text.Length - 2);
                return new PrintStatement(new StringExpression(text));
			}
            else
			{
                throw new NotImplementedException();
			}
        }

        private ProgramLine ProcessForBlock()
        {
            // TODO: Implement for-block processing.
            return null;
        }
    }
}
