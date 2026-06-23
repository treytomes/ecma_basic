using System;

namespace ECMABasic.Domain.Exceptions;

internal class NoEndInstructionException : SyntaxException
{
	public NoEndInstructionException()
		: base("NO END INSTRUCTION")
	{
	}
}
