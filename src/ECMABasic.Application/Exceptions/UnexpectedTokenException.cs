using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using System;

namespace ECMABasic.Application.Exceptions;

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
