using System;

namespace ECMABasic.Domain.Exceptions;

public class NoEndInstructionException : SyntaxException
{
	public NoEndInstructionException()
		: base("NO END INSTRUCTION")
	{
	}
}
