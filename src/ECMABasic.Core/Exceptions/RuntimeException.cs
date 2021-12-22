using System;

namespace ECMABasic.Core.Exceptions
{
	public class RuntimeException : Exception
	{
		public RuntimeException(string message, int? line = null)
			: base(string.Concat($"% {message.ToUpper()}", (line.HasValue ? $" IN LINE {line}" : string.Empty)))
		{
			RootMessage = message;
			LineNumber = line;
		}

		public string RootMessage { get; }
		public int? LineNumber { get; }
	}
}
