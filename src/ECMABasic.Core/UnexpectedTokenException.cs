using System;

namespace ECMABasic.Core
{
	public class UnexpectedTokenException : Exception
	{
		public UnexpectedTokenException(TokenType expected, Token actual)
			: base($"({actual.Line}:{actual.Column}) Expected '{expected}', found '{actual?.Type.ToString() ?? "null"}'")
		{
			ExpectedType = expected;
			ActualToken = actual;
		}

		public TokenType ExpectedType { get; }
		public Token ActualToken { get; }
	}
}
