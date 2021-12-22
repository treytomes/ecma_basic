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

		//protected static IExpression ParseExpression(ComplexTokenReader reader)
		//{
		//	var expr = ParseNumericExpression(reader);
		//	if (expr != null)
		//	{
		//		return expr;
		//	}

		//	expr = ParseStringExpression(reader);
		//	if (expr != null)
		//	{
		//		return expr;
		//	}

		//	throw new SyntaxException("EXPECTED AN EXPRESSION");
		//}

		//protected static IExpression ParseNumericExpression(ComplexTokenReader reader)
		//{
			
		//}

		//protected static IExpression ParseStringExpression(ComplexTokenReader reader)
		//{
			
		//}

		protected static IExpression ProcessExpression(ComplexTokenReader reader)
		{
			return ProcessBinaryExpression(reader);
		}

		protected static IExpression ProcessBinaryExpression(ComplexTokenReader reader)
		{
			var left = ProcessAtomicExpression(reader);
			if (left == null)
			{
				return null;
			}

			ProcessSpace(reader, false);

			var symbol = reader.Next(TokenType.Symbol, false, @"\=|\<|\>"); //\>");
			if (symbol == null)
			{
				return left;
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

			ProcessSpace(reader, false);

			var right = ProcessAtomicExpression(reader);
			if (right == null)
			{
				throw new SyntaxException("EXPECTED AN EXPRESSION");
			}

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

		protected static IExpression ProcessAtomicExpression(ComplexTokenReader reader)
		{
			IExpression valueExpr = ProcessVariableExpression(reader);
			if (valueExpr != null)
			{
				return valueExpr;
			}

			valueExpr = ProcessNumberExpression(reader);
			if (valueExpr != null)
			{
				return valueExpr;
			}

			valueExpr = ProcessStringExpression(reader);
			if (valueExpr != null)
			{
				return valueExpr;
			}

			return null;
		}

		protected static IExpression ProcessNumericalExpression(ComplexTokenReader reader, int? lineNumber = null)
		{
			IExpression valueExpr = ProcessVariableExpression(reader);
			if (valueExpr == null)
			{
				valueExpr = ProcessNumberExpression(reader);
				if (valueExpr == null)
				{
					throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION");
				}
				else
				{
					return valueExpr;
				}
			}
			else
			{
				if ((valueExpr as VariableExpression).IsNumeric)
				{
					return valueExpr;
				}
				else
				{
					throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION", lineNumber);
				}
			}
		}

		protected static VariableExpression ProcessVariableExpression(ComplexTokenReader reader)
		{
			var nameToken = reader.Next(TokenType.Word, false, @"[A-Z][\$\d]?");
			if (nameToken == null)
			{
				return null;
			}

			return new VariableExpression(nameToken.Text);

			//var nameToken = reader.Next(TokenType.NumericVariable, false);
			//if (nameToken == null)
			//{
			//	nameToken = reader.Next(TokenType.StringVariable, false);
			//	if (nameToken == null)
			//	{
			//		return null;
			//	}
			//}
			//return new VariableExpression(nameToken.Text);
		}

		protected static IExpression ProcessNumberExpression(ComplexTokenReader reader)
		{
			var nextValue = reader.NextNumber(false);
			if (!nextValue.HasValue)
			{
				return null;
			}
			return new NumberExpression(nextValue.Value);
		}

		protected static IExpression ProcessStringExpression(ComplexTokenReader reader)
		{
			var valueToken = reader.Next(TokenType.String, false);
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
