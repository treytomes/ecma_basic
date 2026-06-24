using ECMABasic.Domain;
using ECMABasic.Application.Statements;

namespace ECMABasic.Application;

/// <summary>
/// Parses the RANDOMIZE statement per ECMA-55 specification.
/// ECMA55-RND-001: RANDOMIZE generates unpredictable starting point for RND sequence.
/// </summary>
public class RandomizeStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "RANDOMIZE");
		if (token == null)
		{
			return null;
		}

		// RANDOMIZE takes no parameters
		return new RandomizeStatement();
	}
}
