using System;

namespace ECMABasic.Domain.Exceptions;

public class UnexpectedTokenException : Exception
{
	public UnexpectedTokenException(TokenType expected, Token? actual)
		: base($"({actual?.Line ?? 0}:{actual?.Column ?? 0}) Expected '{expected}', found '{actual?.Text ?? "{null}"}'")
	{
		ExpectedType = expected;
		ActualToken = actual!;
	}

	public TokenType ExpectedType { get; }
	public Token ActualToken { get; }
}
