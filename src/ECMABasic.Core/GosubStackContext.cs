namespace ECMABasic.Core
{
	public class GosubStackContext : ICallStackContext
	{
		public GosubStackContext(int lineNumber)
		{
			LineNumber = lineNumber;
		}

		/// <summary>
		/// Return to this line number when the next RETURN is hit.
		/// </summary>
		public int LineNumber { get; }
	}
}
