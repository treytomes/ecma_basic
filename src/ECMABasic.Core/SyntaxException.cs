using System;

namespace ECMABasic.Core
{
	public class SyntaxException : Exception
	{
		public SyntaxException(string message)
			: base(message)
		{
		}
	}
}
