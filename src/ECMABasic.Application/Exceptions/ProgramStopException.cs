using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿namespace ECMABasic.Application.Exceptions;

public class ProgramStopException : RuntimeException
{
	public ProgramStopException(int? lineNumber)
		: base($"STOPPED", lineNumber)
	{
	}
}
