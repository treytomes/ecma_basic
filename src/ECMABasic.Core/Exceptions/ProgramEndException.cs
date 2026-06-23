namespace ECMABasic.Core.Exceptions
{
	internal class ProgramEndException : RuntimeException
	{
		public ProgramEndException(int? lineNumber)
			: base("END", lineNumber)
		{
		}
	}
}
