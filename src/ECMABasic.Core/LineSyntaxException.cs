namespace ECMABasic.Core
{
	public class LineSyntaxException : SyntaxException
	{
		public LineSyntaxException(string message, int line)
			: base($"? {message.ToUpper()} IN LINE {line}")
		{
			RootMessage = message.ToUpper();
			LineNumber = line;
		}

		public string RootMessage { get; }
		public int LineNumber { get; }
	}
}
