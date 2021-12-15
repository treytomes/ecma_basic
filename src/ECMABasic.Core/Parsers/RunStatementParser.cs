using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class RunStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "RUN");
			if (token == null)
			{
				return null;
			}

			if (ProcessSpace(reader, false) == null)
			{
				return new RunStatement(null);
			}

			var lineNumberExpr = ProcessNumericalExpression(reader, lineNumber);

			return new RunStatement(lineNumberExpr);
		}
	}
}
