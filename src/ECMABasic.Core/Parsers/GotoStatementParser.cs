using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class GotoStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			if (reader.Next(TokenType.Word, false, @"GOTO") == null)
			{
				// "GOTO" might be "GO TO".
				if (reader.Next(TokenType.Word, false, @"GO") == null)
				{
					return null;
				}
				else
				{
					ProcessSpace(reader, false);
					reader.Next(TokenType.Word, true, @"TO");
				}
			}

			ProcessSpace(reader, true);

			var lineNumberExpr = ParseNumericalExpression(reader);
			return new GotoStatement(lineNumberExpr);
		}
	}
}
