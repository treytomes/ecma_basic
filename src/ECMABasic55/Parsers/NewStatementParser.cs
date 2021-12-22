using ECMABasic.Core;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers
{
	class NewStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "NEW");
			if (token == null)
			{
				return null;
			}
			return new NewStatement();
		}
	}
}
