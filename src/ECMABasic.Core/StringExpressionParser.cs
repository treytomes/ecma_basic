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
					throw ExceptionFactory.MixedStringsAndNumbers(_lineNumber);
				}
				else
				{
					throw;
				}
			}

			try
			{
				return symbol.Text switch
				{
					"=" => new EqualsExpression(left, right),
					"<>" => new NotEqualsExpression(left, right),
					"<" => new LessThanExpression(left, right),
					"<=" => new LessThanOrEqualExpression(left, right),
					">" => new GreaterThanExpression(left, right),
					">=" => new GreaterThanOrEqualExpression(left, right),
					_ => throw ExceptionFactory.IllegalOperator(_lineNumber),
				};
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
				throw ExceptionFactory.ExpectedStringExpression(_lineNumber);
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
