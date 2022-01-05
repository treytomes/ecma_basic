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

		public override IExpression Parse()
		{
			var expr = ParseBoolean();
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
					throw ExceptionFactory.MixedStringsAndNumbers(_lineNumber);
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
					throw ExceptionFactory.IllegalOperator(_lineNumber);
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

				var preSymbolIndex = _reader.TokenIndex;
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
				if (right == null)
				{
					_reader.Seek(preSymbolIndex);
					return expr;
				}

				expr = symbol.Text switch
				{
					"+" => new AdditionExpression(expr, right),
					"-" => new SubtractionExpression(expr, right),
					_ => throw ExceptionFactory.IllegalOperator(_lineNumber),
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

				var preSymbolIndex = _reader.TokenIndex;
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
				if (right == null)
				{
					_reader.Seek(preSymbolIndex);
					return expr;
				}

				expr = symbol.Text switch
				{
					"*" => new MultiplicationExpression(expr, right),
					"/" => new DivisionExpression(expr, right),
					_ => throw ExceptionFactory.IllegalOperator(_lineNumber),
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

				var preSymbolIndex = _reader.TokenIndex;
				var symbol = _reader.Next(TokenType.Symbol, false, @"\^|\*");
				if (symbol == null)
				{
					if (space != null)
					{
						_reader.Rewind();
					}
					return expr;
				}
				else if (symbol.Text == "*")
				{
					var next = _reader.Next(TokenType.Symbol, false, @"\*");
					if (next == null)
					{
						_reader.Rewind();
						return expr;
					}
					// At this point we've read the "**" operator.
				}

				_reader.Next(TokenType.Space, false);

				var right = ParseAtomic(true);
				if (right == null)
				{
					_reader.Seek(preSymbolIndex);
					return expr;
				}

				expr = new InvolutionExpression(expr, right);
			}
		}

		private IExpression ParseAtomic(bool throwOnError)
		{
			var openParenthesis = _reader.Next(TokenType.OpenParenthesis, false);
			if (openParenthesis != null)
			{
				var expr = Parse();
				try
				{
					_reader.Next(TokenType.CloseParenthesis);
				}
				catch (UnexpectedTokenException)
				{
					throw ExceptionFactory.IllegalFormula(_lineNumber);
				}
				return expr;
			}
			else
			{
				var expr = ParseLiteral() ?? ParseVariable() ?? ParseFunction();
				if (expr == null)
				{
					if (throwOnError)
					{
						throw ExceptionFactory.ExpectedNumericExpression(_lineNumber);
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
				throw ExceptionFactory.UndefinedFunction(_lineNumber);
			}
			else
			{
				return fndef.Instantiate(args, _lineNumber);
			}
		}
	}
}
