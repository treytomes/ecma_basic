using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class NextStatementParser : StatementParser
	{
		public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, @"NEXT");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var loopVar = ParseVariableExpression(reader);
			if (loopVar == null)
			{
				throw new Exceptions.SyntaxException("Expected variable in NEXT statement", lineNumber);
			}

			return new NextStatement(loopVar);
		}
	}
}
