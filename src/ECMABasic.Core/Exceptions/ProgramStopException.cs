namespace ECMABasic.Core.Exceptions
{
	public class ProgramStopException : RuntimeException
	{
		public ProgramStopException(int lineNumber)
			: base($"STOPPED", lineNumber)
		{
		}
	}
}
