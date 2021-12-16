namespace ECMABasic.Core.Exceptions
{
	class ProgramStopException : RuntimeException
	{
		public ProgramStopException(int lineNumber)
			: base($"% STOPPED IN LINE {lineNumber}")
		{
		}
	}
}
