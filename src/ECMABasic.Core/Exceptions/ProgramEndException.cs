namespace ECMABasic.Core.Exceptions
{
	class ProgramEndException : RuntimeException
	{
		public ProgramEndException(int? lineNumber)
			: base("END", lineNumber)
		{
		}
	}
}
