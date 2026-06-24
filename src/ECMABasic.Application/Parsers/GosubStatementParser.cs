using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
using ECMABasic.Domain.Exceptions;
﻿using ECMABasic.Application.Statements;

namespace ECMABasic.Application.Parsers;

public class GosubStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, @"GOSUB");
		if (token == null)
		{
			// "GOSUB" might be "GO SUB".
			token = reader.Next(TokenType.Word, false, @"GO");
			if (token == null)
			{
				return null;
			}
			else
			{
				ProcessSpace(reader, false);
				reader.Next(TokenType.Word, true, @"SUB");
			}
		}

		ProcessSpace(reader, true);

		var lineNumberExpr = ParseNumericExpression(reader, lineNumber, true);
		if (lineNumberExpr == null)
		{
			throw new SyntaxException("Expected line number in GOSUB statement", lineNumber);
		}
		return new GosubStatement(lineNumberExpr);
	}
}
