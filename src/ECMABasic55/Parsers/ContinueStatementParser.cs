using ECMABasic.Core;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers
{
	public class ContinueStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "CONTINUE");
			if (token == null)
			{
				token = reader.Next(TokenType.Word, false, "CONT");
				if (token == null)
				{
					return null;
				}
			}
			return new ContinueStatement();
		}
	}
}
