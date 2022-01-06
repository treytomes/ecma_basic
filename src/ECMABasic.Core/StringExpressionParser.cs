using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System.Collections.Generic;

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
			var expr = ParseLiteral() ?? ParseVariable() ?? ParseFunction();
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

		public IExpression ParseFunction()
		{
			var numberParser = new NumericExpressionParser(_reader, _lineNumber, false);

			var nameToken = _reader.Next(TokenType.Word, false);
			if (nameToken == null)
			{
				return null;
			}

			var dollar = _reader.Next(TokenType.Symbol, false, @"\$");
			if (dollar == null)
			{
				return null;
			}
			nameToken = new Token(TokenType.Word, new[] { nameToken, dollar });

			_reader.Next(TokenType.OpenParenthesis);

			var args = new List<IExpression>();
			while (true)
			{
				var argExpr = Parse();
				if (argExpr == null)
				{
					argExpr = numberParser.Parse();
					if (argExpr == null)
					{
						break;
					}
				}

				args.Add(argExpr);

				var comma = _reader.Next(TokenType.Comma, false);
				if (comma == null)
				{
					break;
				}
			}

			_reader.Next(TokenType.CloseParenthesis);

			foreach (var fndef in FunctionFactory.Instance.Get(nameToken.Text))
			{
				if (fndef.CanInstantiate(args))
				{
					return fndef.Instantiate(args, _lineNumber);
				}
			}
			throw ExceptionFactory.UndefinedFunction(_lineNumber);
		}
	}
}
