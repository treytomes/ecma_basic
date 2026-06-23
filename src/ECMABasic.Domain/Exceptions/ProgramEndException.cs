namespace ECMABasic.Domain.Exceptions;

internal class ProgramEndException : RuntimeException
{
	public ProgramEndException(int? lineNumber)
		: base("END", lineNumber)
	{
	}
}
