using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;

namespace ECMABasic.Core
{
	public class NumericExpressionParser : ExpressionParser
	{
		public NumericExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
			: base(reader, lineNumber, throwOnError)
		{
		}

		// TODO: Boolean expressions need to be their own parser.
		public override IExpression Parse()
		{
			//var unaryMinusToken = _reader.Next(TokenType.Symbol, false, @"\-");
			//if (unaryMinusToken == null)
			//{
			//	// Read the unary plus if it's there, but don't do anything with it.
			//	_reader.Next(TokenType.Symbol, false, @"\+");
			//}

			//var space = _reader.Next(TokenType.Space, false);

			var expr = ParseBoolean();
			//if (expr == null)
			//{
			//	if (space != null)
			//	{
			//		_reader.Rewind();  // Rewind over the space.
			//	}
			//	return null;
			//}

			//if (unaryMinusToken != null)
			//{
			//	expr = new NegationExpression(expr);
			//}
			return expr;
		}

		private IExpression ParseBoolean()
		{
			var left = ParseSums();
			if (left == null)
			{
				return null;
			}

			var space = _reader.Next(TokenType.Space, false);

			var symbol = ParseBooleanOperator();
			if (symbol == null)
			{
				if (space != null)
				{
					_reader.Rewind();
				}
				return left;
			}

			_reader.Next(TokenType.Space, false);


			IExpression right;
			try
			{
				right = ParseSums();
			}
			catch (SyntaxException)
			{
				if (new StringExpressionParser(_reader, _lineNumber, false).Parse() != null)
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

		private IExpression ParseSums()
		{
			var expr = ParseProducts();
			if (expr == null)
			{
				return expr;
			}

			while (true)
			{
				var space = _reader.Next(TokenType.Space, false);

				var symbol = _reader.Next(TokenType.Symbol, false, @"\+|\-");
				if (symbol == null)
				{
					if (space != null)
					{
						_reader.Rewind();
					}
					return expr;
				}

				_reader.Next(TokenType.Space, false);

				var right = ParseProducts();

				expr = symbol.Text switch
				{
					"+" => new AdditionExpression(expr, right),
					"-" => new SubtractionExpression(expr, right),
					_ => throw new InvalidOperationException(),
				};
			}
		}

		private IExpression ParseProducts()
		{
			var expr = ParseUnary();
			if (expr == null)
			{
				return expr;
			}

			while (true)
			{
				var space = _reader.Next(TokenType.Space, false);

				var symbol = _reader.Next(TokenType.Symbol, false, @"\*|\/");
				if (symbol == null)
				{
					if (space != null)
					{
						_reader.Rewind();
					}
					return expr;
				}

				_reader.Next(TokenType.Space, false);

				var right = ParseUnary();

				expr = symbol.Text switch
				{
					"*" => new MultiplicationExpression(expr, right),
					"/" => new DivisionExpression(expr, right),
					_ => throw new InvalidOperationException(),
				};
			}
		}

		private IExpression ParseUnary()
		{
			var unaryMinusToken = _reader.Next(TokenType.Symbol, false, @"\-");
			if (unaryMinusToken == null)
			{
				// Read the unary plus if it's there, but don't do anything with it.
				_reader.Next(TokenType.Symbol, false, @"\+");
			}

			_reader.Next(TokenType.Space, false);

			var expr = ParseInvolution();
			if (unaryMinusToken != null)
			{
				expr = new NegationExpression(expr);
			}

			return expr;
		}

		private IExpression ParseInvolution()
		{
			var expr = ParseAtomic(false);
			if (expr == null)
			{
				return expr;
			}

			while (true)
			{
				var space = _reader.Next(TokenType.Space, false);

				var symbol = _reader.Next(TokenType.Symbol, false, @"\^");
				if (symbol == null)
				{
					if (space != null)
					{
						_reader.Rewind();
					}
					return expr;
				}

				_reader.Next(TokenType.Space, false);

				var right = ParseAtomic(true);

				expr = new InvolutionExpression(expr, right);
			}
		}

		private IExpression ParseAtomic(bool throwOnError)
		{
			var openParenthesis = _reader.Next(TokenType.OpenParenthesis, false);
			if (openParenthesis != null)
			{
				var expr = Parse();
				_reader.Next(TokenType.CloseParenthesis);
				return expr;
			}
			else
			{
				var expr = ParseLiteral() ?? ParseVariable() ?? ParseFunction();
				if (expr == null)
				{
					if (throwOnError)
					{
						throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION", _lineNumber);
					}
					else
					{
						return expr;
					}
				}

				return expr;
			}
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

		public IExpression ParseFunction()
		{
			var nameToken = _reader.Next(TokenType.Word, false);
			if (nameToken == null)
			{
				return null;
			}

			_reader.Next(TokenType.OpenParenthesis);

			var args = new List<IExpression>();
			while (true)
			{
				args.Add(Parse());

				var comma = _reader.Next(TokenType.Comma, false);
				if (comma == null)
				{
					break;
				}
			}

			_reader.Next(TokenType.CloseParenthesis);

			var fndef = FunctionFactory.Instance.Get(nameToken.Text);
			if (fndef == null)
			{
				throw new SyntaxException("UNDEFINED FUNCTION");
			}
			else
			{
				return fndef.Instantiate(args);
			}
		}
	}
}
