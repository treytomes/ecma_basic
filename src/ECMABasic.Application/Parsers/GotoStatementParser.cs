using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Statements;

namespace ECMABasic.Application.Parsers;

public class GotoStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		if (reader.Next(TokenType.Word, false, @"GOTO") == null)
		{
			// "GOTO" might be "GO TO".
			if (reader.Next(TokenType.Word, false, @"GO") == null)
			{
				return null;
			}
			else
			{
				ProcessSpace(reader, false);
				reader.Next(TokenType.Word, true, @"TO");
			}
		}

		ProcessSpace(reader, true);

		var lineNumberExpr = ParseNumericExpression(reader, lineNumber, true);
		if (lineNumberExpr == null)
		{
			throw new Exceptions.SyntaxException("Expected line number in GOTO statement", lineNumber);
		}
		return new GotoStatement(lineNumberExpr);
	}
}
