using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Exceptions;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers;

public class SaveStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "SAVE");
		if (token == null)
		{
			return null;
		}

		ProcessSpace(reader);

		var filenameExpr = ParseStringExpression(reader, null, true);
		if (filenameExpr == null)
		{
			throw ECMABasic.Application.ExceptionFactory.Syntax();
		}

		return new SaveStatement(filenameExpr);
	}
}
