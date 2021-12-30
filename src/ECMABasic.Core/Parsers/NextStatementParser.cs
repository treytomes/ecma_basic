using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class NextStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, @"NEXT");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var loopVar = ParseVariableExpression(reader);

			return new NextStatement(loopVar);
		}
	}
}
