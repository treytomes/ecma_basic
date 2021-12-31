using ECMABasic.Core;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers
{
	public class SleepStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			if (reader.Next(TokenType.Word, false, @"SLEEP") == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var milliseconds = ParseNumericExpression(reader, lineNumber, true);
			return new SleepStatement(milliseconds);
		}
	}
}
