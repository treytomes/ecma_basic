namespace ECMABasic.Domain.Exceptions;

public class ProgramStopException : RuntimeException
{
	public ProgramStopException(int? lineNumber)
		: base($"STOPPED", lineNumber)
	{
	}
}
