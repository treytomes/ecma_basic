using ECMABasic.Core;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers
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

			var lineNumberExpr = ParseNumericExpression(reader, lineNumber, false);

			return new RunStatement(lineNumberExpr);
		}
	}
}
