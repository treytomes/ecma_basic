using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	class LoadStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "LOAD");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader);

			var filenameExpr = ProcessStringExpression(reader);

			return new LoadStatement(filenameExpr);
		}
	}
}
