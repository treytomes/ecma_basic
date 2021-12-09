namespace ECMABasic.Core.Exceptions
{
	public class LineRuntimeException : RuntimeException
	{
		public LineRuntimeException(string message, int line)
			: base($"% {message.ToUpper()} IN LINE {line}")
		{
			RootMessage = message.ToUpper();
			LineNumber = line;
		}

		public string RootMessage { get; }
		public int LineNumber { get; }
	}
}
