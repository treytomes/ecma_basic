using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿namespace ECMABasic.Application.Exceptions;

internal class ProgramEndException : RuntimeException
{
	public ProgramEndException(int? lineNumber)
		: base("END", lineNumber)
	{
	}
}
