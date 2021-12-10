namespace ECMABasic.Core
{
	/// <summary>
	/// Represents a thing that can be sent to the PRINT statement.
	/// </summary>
	/// <remarks>
	/// A print-item may not be a valid expression component.  e.g. TAB.
	/// </remarks>
	public interface IPrintItem
	{
		/// <summary>
		/// Convert the expression a base type.
		/// </summary>
		/// <param name="env">The environment to evaluate against.</param>
		/// <returns>The text representation of this expression.</returns>
		object Evaluate(IEnvironment env);
	}
}
