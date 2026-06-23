using System;

namespace ECMABasic.Core.Exceptions
{
	internal class NoEndInstructionException : SyntaxException
	{
		public NoEndInstructionException()
			: base("NO END INSTRUCTION")
		{
		}
	}
}
