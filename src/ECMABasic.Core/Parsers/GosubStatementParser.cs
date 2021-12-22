using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class GosubStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, @"GOSUB");
			if (token == null)
			{
				// "GOSUB" might be "GO SUB".
				token = reader.Next(TokenType.Word, false, @"GO");
				if (token == null)
				{
					return null;
				}
				else
				{
					ProcessSpace(reader, false);
					reader.Next(TokenType.Word, true, @"SUB");
				}
			}

			ProcessSpace(reader, true);

			var lineNumberExpr = ParseNumericalExpression(reader, lineNumber, true);
			return new GosubStatement(lineNumberExpr);
		}
	}
}
