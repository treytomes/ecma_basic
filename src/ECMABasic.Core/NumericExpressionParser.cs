using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public class NumericExpressionParser : ExpressionParser
	{
		public NumericExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
			: base(reader, lineNumber, throwOnError)
		{
		}

		public IExpression ParseBinary()
		{
			var left = ParseAtomic(false);
			if (left == null)
			{
				return null;
			}

			_reader.Next(TokenType.Space, false);

			var symbol = ParseOperator();
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
				if (new StringExpressionParser(_reader, _lineNumber, false).ParseBinary() != null)
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
				else if (symbol.Text == "+")
				{
					return new AdditionExpression(left, right);
				}
				else if (symbol.Text == "-")
				{
					return new SubtractionExpression(left, right);
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
			var unaryMinusToken = _reader.Next(TokenType.Symbol, false, @"\-");

			var expr = ParseLiteral() ?? ParseVariable();
			if ((expr == null) && throwOnError)
			{
				throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION", _lineNumber);
			}

			if (unaryMinusToken != null)
			{
				if (expr is NumberExpression)
				{
					expr = (expr as NumberExpression).Negate();
				}
				else
				{
					expr = new NegationExpression(expr);
				}
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
}
