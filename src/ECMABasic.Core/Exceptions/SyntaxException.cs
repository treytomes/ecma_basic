using System;

namespace ECMABasic.Core.Exceptions
{
	public class SyntaxException : RuntimeException
	{
		public SyntaxException(string message, int? line = null)
			: base(string.Concat($"? {message.ToUpper()}", (line.HasValue ? $" IN LINE {line}" : string.Empty)))
		{
			RootMessage = message.ToUpper();
			LineNumber = line;
		}

		public string RootMessage { get; }
		public int? LineNumber { get; }
	}
}
