using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class ContinueStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "CONTINUE");
			if (token == null)
			{
				return null;
			}
			return new ContinueStatement();
		}
	}
}
