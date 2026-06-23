using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;

namespace ECMABasic.Core.Parsers
{
	public class ForStatementParser : StatementParser
	{
		public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, @"FOR");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var loopVar = ParseVariableExpression(reader);
			if (loopVar == null)
			{
				throw new Exceptions.SyntaxException("Expected variable in FOR statement", lineNumber);
			}

			ProcessSpace(reader, false);
			reader.Next(TokenType.Symbol, true, @"\=");
			ProcessSpace(reader, false);

			var from = ParseNumericExpression(reader, lineNumber, true);
			if (from == null)
			{
				throw new Exceptions.SyntaxException("Expected FROM expression in FOR statement", lineNumber);
			}

			ProcessSpace(reader, true);
			reader.Next(TokenType.Word, true, "TO");
			ProcessSpace(reader, true);

			var to = ParseNumericExpression(reader, lineNumber, true);
			if (to == null)
			{
				throw new Exceptions.SyntaxException("Expected TO expression in FOR statement", lineNumber);
			}

			IExpression step;
			ProcessSpace(reader, false);
			token = reader.Next(TokenType.Word, false, "STEP");
			if (token != null)
			{
				ProcessSpace(reader, true);
				var stepExpr = ParseNumericExpression(reader, lineNumber, true);
				if (stepExpr == null)
				{
					throw new Exceptions.SyntaxException("Expected STEP expression in FOR statement", lineNumber);
				}
				step = stepExpr;
			}
			else
			{
				step = new NumberExpression(1);
			}
			return new ForStatement(loopVar, from, to, step);
		}
	}
}
