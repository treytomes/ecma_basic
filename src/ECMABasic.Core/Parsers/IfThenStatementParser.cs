using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			var conditionExpr = ProcessExpression(reader);
			if (conditionExpr == null)
			{
				throw new SyntaxException("EXPECTED AN EXPRESSION");
			}

			ProcessSpace(reader, true);

			reader.Next(TokenType.Word, true, @"THEN");

			ProcessSpace(reader, true);

			var lineNumberExpr = ProcessExpression(reader);
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
