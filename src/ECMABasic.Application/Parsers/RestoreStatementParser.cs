using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Statements;

namespace ECMABasic.Application.Parsers;

public class RestoreStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "RESTORE");
		if (token == null)
		{
			return null;
		}
		return new RestoreStatement();
	}
}
