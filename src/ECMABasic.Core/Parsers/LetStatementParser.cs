using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;

namespace ECMABasic.Core
{
	public class LetStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			//if (lineNumber == 120)
			//{
			//	var a = 0;
			//}
			var token = reader.Next(TokenType.Word, false, "LET");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var targetExpr = ParseVariableExpression(reader);
			if (targetExpr == null)
			{
				throw new SyntaxException("EXPECTED A VARIABLE NAME", lineNumber);
			}

			ProcessSpace(reader, false);

			reader.Next(TokenType.Symbol, true, @"\=");

			ProcessSpace(reader, false);

			var valueExpr = ParseExpression(reader);
			if (valueExpr == null)
			{
				throw new SyntaxException("EXPECTED AN EXPRESSION", lineNumber);
			}

			return new LetStatement(targetExpr, valueExpr);
		}
	}
}
