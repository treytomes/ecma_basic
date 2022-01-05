using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;

namespace ECMABasic.Core
{
	public class LetStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "LET");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var targetExpr = ParseVariableExpression(reader);
			if (targetExpr == null)
			{
				throw ExceptionFactory.ExpectedVariable(lineNumber);
			}

			ProcessSpace(reader, false);

			reader.Next(TokenType.Symbol, true, @"\=");

			ProcessSpace(reader, false);

			IExpression valueExpr;
			if (targetExpr.IsNumeric)
			{
				valueExpr = ParseNumericExpression(reader, lineNumber, true);
			}
			else
			{
				valueExpr = ParseStringExpression(reader, lineNumber, true);
			}

			return new LetStatement(targetExpr, valueExpr);
		}
	}
}
