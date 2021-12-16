namespace ECMABasic.Core
{
	public interface IListable
	{
        /// <summary>
        /// Output a string formatted for the output of a LIST statement.
        /// </summary>
        /// <returns>The textual representation of this item.</returns>
        string ToListing();
    }
}
