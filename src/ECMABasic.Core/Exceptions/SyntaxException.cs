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

		public SyntaxException(SyntaxException ex, int? line = null)
			: base(string.Concat(ex.Message, (line.HasValue ? $" IN LINE {line}" : string.Empty)))
		{
			RootMessage = ex.RootMessage.ToUpper();
			LineNumber = line;
		}

		public string RootMessage { get; }
		public int? LineNumber { get; }
	}
}
