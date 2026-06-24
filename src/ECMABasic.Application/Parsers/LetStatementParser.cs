using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Exceptions;
using ECMABasic.Application.Statements;

namespace ECMABasic.Application;

public class LetStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "LET");
		if (token == null)
		{
			return null;
		}

		ProcessSpace(reader, true);

		var targetExpr = ParseVariableExpression(reader);
		if (targetExpr == null)
		{
			throw ExceptionFactory.ExpectedVariable(lineNumber);
		}

		ProcessSpace(reader, false);

		reader.Next(TokenType.Symbol, true, @"\=");

		ProcessSpace(reader, false);

		var valueExpr = targetExpr.IsNumeric
			? ParseNumericExpression(reader, lineNumber, true)
			: ParseStringExpression(reader, lineNumber, true);

		if (valueExpr == null)
		{
			throw new Exceptions.SyntaxException("Expected expression in LET statement", lineNumber);
		}

		return new LetStatement(targetExpr, valueExpr);
	}
}
