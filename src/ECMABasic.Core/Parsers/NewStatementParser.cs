using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
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
