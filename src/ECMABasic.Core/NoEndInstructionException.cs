using System;

namespace ECMABasic.Core
{
	class NoEndInstructionException : SyntaxException
	{
		public NoEndInstructionException()
			: base("? NO END INSTRUCTION")
		{
		}
	}
}
