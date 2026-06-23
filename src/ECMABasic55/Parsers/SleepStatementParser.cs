using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Core;
using ECMABasic.Core.Exceptions;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers;

public class SleepStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		if (reader.Next(TokenType.Word, false, @"SLEEP") == null)
		{
			return null;
		}

		ProcessSpace(reader, true);

		var milliseconds = ParseNumericExpression(reader, lineNumber, true);
		if (milliseconds == null)
		{
			throw ECMABasic.Core.ExceptionFactory.Syntax();
		}
		return new SleepStatement(milliseconds);
	}
}
