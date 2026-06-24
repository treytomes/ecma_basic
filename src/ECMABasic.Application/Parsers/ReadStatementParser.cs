using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
using ECMABasic.Domain.Exceptions;
using ECMABasic.Application.Statements;
using System.Collections.Generic;

namespace ECMABasic.Application.Parsers;

public class ReadStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var token = reader.Next(TokenType.Word, false, "READ");
		if (token == null)
		{
			return null;
		}
		ProcessSpace(reader, true);

		var firstVar = ParseVariableExpression(reader);
		if (firstVar == null)
		{
			throw new SyntaxException("Expected variable in READ statement", lineNumber);
		}

		var vars = new List<VariableExpression> { firstVar };

		while (true)
		{
			if (reader.Next(TokenType.Comma, false) == null)
			{
				break;
			}
			ProcessSpace(reader, false);
			var nextVar = ParseVariableExpression(reader);
			if (nextVar != null)
			{
				vars.Add(nextVar);
			}
		}

		return new ReadStatement(vars);
	}
}
