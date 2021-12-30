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
			var expr = new StringExpressionParser(reader, lineNumber, throwOnError).Parse();
			if (expr == null)
			{
				expr = new NumericExpressionParser(reader, lineNumber, throwOnError).Parse();
			}
			return expr;
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
			return new NumericExpressionParser(reader, lineNumber, throwOnError).Parse();
		}

		protected static IExpression ParseStringExpression(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			return new StringExpressionParser(reader, lineNumber, throwOnError).Parse();
		}
	}
}
