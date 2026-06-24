namespace ECMABasic.Domain.Exceptions;

public class ProgramEndException : RuntimeException
{
	public ProgramEndException(int? lineNumber)
		: base("END", lineNumber)
	{
	}
}
