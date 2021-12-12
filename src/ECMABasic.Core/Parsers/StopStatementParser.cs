using ECMABasic.Core.Statements;

namespace ECMABasic.Core
{
	public class StopStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "STOP");
			if (token == null)
			{
				return null;
			}
			return new StopStatement();
		}
	}
}
