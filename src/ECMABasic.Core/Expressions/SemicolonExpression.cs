namespace ECMABasic.Core.Expressions
{
	/// <summary>
	/// Used when a semi-colon occurs in a print list.
	/// </summary>
	public class SemicolonExpression : IPrintItemSeparator
	{
		public object Evaluate(IEnvironment env)
		{
			// Nothing really to do here.  Printing will naturally move to the next column on it's own.
			return string.Empty;
		}

		public string ToListing()
		{
			return ";";
		}
	}
}
