using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Statements;

namespace ECMABasic.Application.Parsers;

public class ReturnStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "RETURN");
		if (token == null)
		{
			return null;
		}
		return new ReturnStatement();
	}
}
