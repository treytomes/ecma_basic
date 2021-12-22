using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;

namespace ECMABasic.Core
{
	public class NumericExpressionParser
	{
		private readonly ComplexTokenReader _reader;
		private readonly int? _lineNumber;
		private readonly bool _throwOnError;

		public NumericExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
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
				throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION", _lineNumber);
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
