namespace ECMABasic.Core
{
	/// <summary>
	/// All of the token types understood by the language.
	/// </summary>
	public enum TokenType
	{
		/// <summary>
		/// [0-9]+.
		/// </summary>
		Integer,

		/// <summary>
		/// [A-Z]+
		/// </summary>
		Word,

		/// <summary>
		/// [!"%&'()*+,-./:;<=>?_#$#@[\\]^`{|}~]+
		/// </summary>
		Symbol,

		/// <summary>
		/// [ \t]+
		/// </summary>
		Space,

		/// <summary>
		/// \x0A\x0D?
		/// </summary>
		EndOfLine,

		/// <summary>
		/// Anything sitting between 2 double-quotes, inclusive.
		/// </summary>
		String,

		/// <summary>
		/// The END keyword.
		/// </summary>
		Keyword_END,

		/// <summary>
		/// The PRINT keyword.
		/// </summary>
		Keyword_PRINT,

		/// <summary>
		/// The STOP keyword.
		/// </summary>
		Keyword_STOP
	}
}
