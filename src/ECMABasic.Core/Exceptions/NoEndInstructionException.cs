using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using System;

namespace ECMABasic.Application.Exceptions;

internal class NoEndInstructionException : SyntaxException
{
	public NoEndInstructionException()
		: base("NO END INSTRUCTION")
	{
	}
}
