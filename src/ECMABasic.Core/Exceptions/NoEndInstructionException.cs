using System;

namespace ECMABasic.Core.Exceptions
{
	class NoEndInstructionException : SyntaxException
	{
		public NoEndInstructionException()
			: base("NO END INSTRUCTION")
		{
		}
	}
}
