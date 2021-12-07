using System;

namespace ECMABasic.Core
{
	class NoEndInstructionException : Exception
	{
		public NoEndInstructionException()
			: base("? NO END INSTRUCTION")
		{
		}
	}
}
