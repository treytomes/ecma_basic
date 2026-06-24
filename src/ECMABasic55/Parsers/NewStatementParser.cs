using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic55.Statements;

namespace ECMABasic55.Parsers;

internal class NewStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "NEW");
		if (token == null)
		{
			return null;
		}
		return new NewStatement();
	}
}
