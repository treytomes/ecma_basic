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
		protected static IExpression ProcessExpression(ComplexTokenReader reader)
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
					throw new SyntaxException("EXPECTED A TAB ARGUMENT");
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
			var valueToken = reader.Next(TokenType.NumericVariable, false);
			if (valueToken == null)
			{
				valueToken = reader.Next(TokenType.StringVariable, false);
				if (valueToken == null)
				{
					return null;
				}
			}
			return new VariableExpression(valueToken.Text);
		}

		protected static IExpression ProcessNumberExpression(ComplexTokenReader reader)
		{
			var isNegative = false;

			var signToken = reader.Next(TokenType.Symbol, false, "-");
			if (signToken != null)
			{
				isNegative = true;
			}
			else
			{
				reader.Next(TokenType.Symbol, false, "+");
				// If a positive sign is found, no action is needed.
			}

			var tabValue = reader.NextNumber(false);
			if (!tabValue.HasValue)
			{
				return null;
			}

			var value = tabValue.Value;
			if (isNegative)
			{
				value = -value;
			}

			return new NumberExpression(value);
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
