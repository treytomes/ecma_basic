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
		Digit,

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

		// Complex tokens start here.

		/// <summary>
		/// Anything sitting between 2 double-quotes, inclusive.
		/// </summary>
		String,

		/// <summary>
		/// The END keyword.
		/// </summary>
		Keyword_END,

		/// <summary>
		/// The LET keyword.
		/// </summary>
		Keyword_LET,

		/// <summary>
		/// The PRINT keyword.
		/// </summary>
		Keyword_PRINT,

		/// <summary>
		/// The STOP keyword.
		/// </summary>
		Keyword_STOP,

		/// <summary>
		/// Represents the TAB function in a print-list.
		/// </summary>
		Keyword_TAB,

		/// <summary>
		/// The comma in a print-list.
		/// </summary>
		Comma,

		/// <summary>
		/// The semicolon in a print-list.
		/// </summary>
		Semicolon,

		/// <summary>
		/// Opening parenthesis to a function argument list.
		/// </summary>
		OpenParenthesis,

		/// <summary>
		/// closing parenthesis to a function argument list.
		/// </summary>
		CloseParenthesis,

		/// <summary>
		/// The equals sign used in comparison and assignment.
		/// </summary>
		Equals,

		/// <summary>
		/// A string variable name: [A-Z]\$
		/// </summary>
		StringVariable,

		/// <summary>
		/// A numeric variable name: [A-Z][0-9]?
		/// </summary>
		NumericVariable,

		/// <summary>
		/// The minus sign, either binary or unary.
		/// </summary>
		Negation,

		/// <summary>
		/// A real number: [0-9]*\.[0-9]+
		/// </summary>
		Number,

		/// <summary>
		/// Breaks up a number.
		/// </summary>
		DecimalPoint
	}
}
