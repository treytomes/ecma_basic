using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public abstract class StatementParser
	{
		/// <summary>
		/// Parse a statement off of the next series of tokens.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="lineNumber">The current line number.  Null for immediate evaluation.</param>
		/// <returns>The executable statement.</returns>
		public abstract IStatement Parse(ComplexTokenReader reader, int? lineNumber = null);

		/// <summary>
		/// Read whitespace off of the token stream.
		/// An exception will optionally occur if whitespace could not be found.
		/// </summary>
		/// <param name="throwOnError">Throw an exception if space is not found.  Default to true.</param>
		/// <returns>The space token.</returns>
		protected static Token ProcessSpace(ComplexTokenReader reader, bool throwOnError = true)
		{
			return reader.Next(TokenType.Space, throwOnError);
		}

		protected static IExpression ParseExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			return ParseBinaryExpression(reader, lineNumber, throwOnError);
		}

		protected static IExpression ParseBinaryExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			var left = ParseAtomicExpression(reader, lineNumber, throwOnError);
			if (left == null)
			{
				return null;
			}

			ProcessSpace(reader, false);

			var symbol = ParseOperator(reader, lineNumber, throwOnError);
			if (symbol == null)
			{
				return left;
			}

			ProcessSpace(reader, false);

			var right = ParseAtomicExpression(reader, lineNumber, true);

			try
			{
				if (symbol.Text == "=")
				{
					return new EqualsExpression(left, right);
				}
				else if (symbol.Text == "<>")
				{
					return new NotEqualsExpression(left, right);
				}
				else if (symbol.Text == "<")
				{
					return new LessThanExpression(left, right);
				}
				else if (symbol.Text == "<=")
				{
					return new LessThanOrEqualExpression(left, right);
				}
				else if (symbol.Text == ">")
				{
					return new GreaterThanExpression(left, right);
				}
				else if (symbol.Text == ">=")
				{
					return new GreaterThanOrEqualExpression(left, right);
				}
				else
				{
					throw new UnexpectedTokenException(TokenType.Symbol, symbol);
				}
			}
			catch (SyntaxException ex)
			{
				throw new SyntaxException(ex, lineNumber);
			}
		}

		private static Token ParseOperator(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			var symbol = reader.Next(TokenType.Symbol, false, @"\=|\<|\>"); //\>");
			if (symbol == null)
			{
				return null;
			}

			if (symbol.Text == "<")
			{
				var nextBit = reader.Next(TokenType.Symbol, false, @"\=|\>");
				if (nextBit != null)
				{
					if (nextBit.Text == ">")
					{
						symbol = new Token(TokenType.Symbol, new[] { symbol, nextBit });  // Finish out the inequality operator.
					}
					else if (nextBit.Text == "=")
					{
						symbol = new Token(TokenType.Symbol, new[] { symbol, nextBit });  // Finish out the >= operator.
					}
				}
			}
			else if (symbol.Text == ">")
			{
				var nextBit = reader.Next(TokenType.Symbol, false, @"\=");
				if (nextBit != null)
				{
					if (nextBit.Text == "=")
					{
						symbol = new Token(TokenType.Symbol, new[] { symbol, nextBit });  // Finish out the <= operator.
					}
				}
			}

			return symbol;
		}

		protected static IExpression ParseAtomicExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			var valueExpr = ParseNumericExpression(reader, lineNumber, false);
			if (valueExpr != null)
			{
				return valueExpr;
			}

			valueExpr = ParseStringExpression(reader, lineNumber, throwOnError);
			if (valueExpr != null)
			{
				return valueExpr;
			}

			return null;
		}

		/// <summary>
		/// This will parse either a numeric variable or a string variable.
		/// </summary>
		/// <param name="reader">The tokenizer to pull the variable text from.</param>
		/// <returns>The variable expression.</returns>
		protected static VariableExpression ParseVariableExpression(ComplexTokenReader reader)
		{
			var nameToken = reader.Next(TokenType.Word, false, @"[A-Z][\$\d]?");
			if (nameToken == null)
			{
				return null;
			}

			return new VariableExpression(nameToken.Text);
		}

		protected static IExpression ParseNumericExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			return new NumericExpressionParser(reader, lineNumber, throwOnError).ParseAtomic();
		}

		protected static IExpression ParseStringExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			return new StringExpressionParser(reader, lineNumber, throwOnError).ParseAtomic();
		}
	}

	public class NumericExpressionParser
	{
		private readonly ComplexTokenReader _reader;
		private readonly int? _lineNumber;
		private readonly bool _throwOnError;

		public NumericExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			_reader = reader;
			_lineNumber = lineNumber;
			_throwOnError = throwOnError;
		}

		public IExpression ParseAtomic()
		{
			var expr = ParseLiteral() ?? ParseVariable();
			if ((expr == null) && _throwOnError)
			{
				throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION", _lineNumber);
			}
			return expr;
		}

		public IExpression ParseVariable()
		{
			var nameToken = _reader.Next(TokenType.Word, false, @"[A-Z]\d?");
			if (nameToken == null)
			{
				return null;
			}

			return new VariableExpression(nameToken.Text);
		}

		public IExpression ParseLiteral()
		{
			var nextValue = _reader.NextNumber(false);
			if (!nextValue.HasValue)
			{
				return null;
			}
			return new NumberExpression(nextValue.Value);
		}
	}

	public class StringExpressionParser
	{
		private readonly ComplexTokenReader _reader;
		private readonly int? _lineNumber;
		private readonly bool _throwOnError;

		public StringExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			_reader = reader;
			_lineNumber = lineNumber;
			_throwOnError = throwOnError;
		}

		public IExpression ParseAtomic()
		{
			var expr = ParseLiteral() ?? ParseVariable();
			if ((expr == null) && _throwOnError)
			{
				throw new SyntaxException("EXPECTED A STRING EXPRESSION", _lineNumber);
			}
			return expr;
		}

		public IExpression ParseVariable()
		{
			var nameToken = _reader.Next(TokenType.Word, false, @"[A-Z]\$");
			if (nameToken == null)
			{
				return null;
			}

			return new VariableExpression(nameToken.Text);
		}

		public IExpression ParseLiteral()
		{
			var valueToken = _reader.Next(TokenType.String, false);
			if (valueToken == null)
			{
				return null;
			}
			else
			{
				// The actual string is everything between the "".
				var text = valueToken.Text[1..^1];
				return new StringExpression(text);
			}
		}
	}
}
