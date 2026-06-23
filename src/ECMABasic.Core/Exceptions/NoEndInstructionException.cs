using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using System;

namespace ECMABasic.Core.Exceptions;

internal class NoEndInstructionException : SyntaxException
{
	public NoEndInstructionException()
		: base("NO END INSTRUCTION")
	{
	}
}
