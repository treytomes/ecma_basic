using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Statements;

namespace ECMABasic.Application;

public class EndStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "END");
		if (token == null)
		{
			return null;
		}
		return new EndStatement();
	}
}
