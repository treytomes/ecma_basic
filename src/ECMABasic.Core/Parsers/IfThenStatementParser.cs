using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;
using System;

namespace ECMABasic.Core.Parsers
{
	public class IfThenStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			if (reader.Next(TokenType.Word, false, @"IF") == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			var conditionExpr = ParseExpression(reader);
			if (conditionExpr == null)
			{
				throw new SyntaxException("EXPECTED AN EXPRESSION");
			}

			ProcessSpace(reader, true);

			reader.Next(TokenType.Word, true, @"THEN");

			ProcessSpace(reader, true);

			var lineNumberExpr = ParseExpression(reader);
			if (lineNumberExpr == null)
			{
				throw new SyntaxException("EXPECTED AN EXPRESSION");
			}

			return new IfThenStatement(conditionExpr, lineNumberExpr);
		}

		private IExpression ParseCondition(ComplexTokenReader reader, int? lineNumber)
		{
			throw new NotImplementedException();
		}
	}
}
