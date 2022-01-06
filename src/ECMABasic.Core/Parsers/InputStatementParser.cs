using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;
using System.Collections.Generic;

namespace ECMABasic.Core.Parsers
{
	public class InputStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "INPUT");
			if (token == null)
			{
				return null;
			}
			ProcessSpace(reader, true);

			var vars = new List<VariableExpression>
			{
				ParseVariableExpression(reader)
			};

			while (true)
			{
				if (reader.Next(TokenType.Comma, false) == null)
				{
					break;
				}
				ProcessSpace(reader, false);
				vars.Add(ParseVariableExpression(reader));
			}

			return new InputStatement(vars);
		}
	}
}
