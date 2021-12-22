using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
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
