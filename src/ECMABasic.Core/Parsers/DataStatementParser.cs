using ECMABasic.Core.Statements;
using System.Collections.Generic;

namespace ECMABasic.Core.Parsers
{
	public class DataStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "DATA");
			if (token == null)
			{
				return null;
			}
			ProcessSpace(reader, true);

			var datums = new List<IExpression>
			{
				ParseExpression(reader, lineNumber, true)
			};

			while (true)
			{
				if (reader.Next(TokenType.Comma, false) == null)
				{
					break;
				}
				ProcessSpace(reader, false);
				datums.Add(ParseExpression(reader, lineNumber, true));
			}

			return new DataStatement(datums);
		}
	}
}
