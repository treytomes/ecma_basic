namespace ECMABasic.Core
{
	public class ExpressionParser
	{
		protected readonly ComplexTokenReader _reader;
		protected readonly int? _lineNumber;
		protected readonly bool _throwOnError;

		protected ExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		{
			_reader = reader;
			_lineNumber = lineNumber;
			_throwOnError = throwOnError;
		}

		protected Token ParseOperator()
		{
			var symbol = _reader.Next(TokenType.Symbol, false, @"\=|\<|\>|\+|\-"); //\>");
			if (symbol == null)
			{
				return null;
			}

			if (symbol.Text == "<")
			{
				var nextBit = _reader.Next(TokenType.Symbol, false, @"\=|\>");
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
				var nextBit = _reader.Next(TokenType.Symbol, false, @"\=");
				if (nextBit != null)
				{
					if (nextBit.Text == "=")
					{
						symbol = new Token(TokenType.Symbol, new[] { symbol, nextBit });  // Finish out the <= operator.
					}
				}
			}

			return symbol;
		}
	}
}
