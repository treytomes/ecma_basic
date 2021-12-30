using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public class StringExpressionParser : ExpressionParser
	{
		public StringExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
			: base(reader, lineNumber, throwOnError)
		{
		}

		public override IExpression Parse()
		{
			var left = ParseAtomic(false);
			if (left == null)
			{
				return null;
			}

			_reader.Next(TokenType.Space, false);

			var symbol = ParseBooleanOperator();
			if (symbol == null)
			{
				return left;
			}

			_reader.Next(TokenType.Space, false);

			IExpression right;
			try
			{
				right = ParseAtomic(true);
			}
			catch (SyntaxException)
			{
				if (new NumericExpressionParser(_reader, _lineNumber, false).Parse() != null)
				{
					throw new SyntaxException("MIXED STRINGS AND NUMBERS", _lineNumber);
				}
				else
				{
					throw;
				}
			}

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
				throw new SyntaxException(ex, _lineNumber);
			}
		}

		private IExpression ParseAtomic(bool throwOnError)
		{
			var expr = ParseLiteral() ?? ParseVariable();
			if ((expr == null) && throwOnError)
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
