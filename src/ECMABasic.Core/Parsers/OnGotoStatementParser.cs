using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Parsers
{
	public class OnGotoStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			if (reader.Next(TokenType.Word, false, @"ON") == null)
			{
				return null;
			}

			ProcessSpace(reader, true);
			var value = ParseNumericExpression(reader, lineNumber, true);
			ProcessSpace(reader, true);

			if (reader.Next(TokenType.Word, false, @"GOTO") == null)
			{
				// "GOTO" might be "GO TO".
				if (reader.Next(TokenType.Word, false, @"GO") == null)
				{
					return null;
				}
				else
				{
					ProcessSpace(reader, false);
					reader.Next(TokenType.Word, true, @"TO");
				}
			}

			ProcessSpace(reader, true);

			var branches = new List<IExpression>();

			while (true)
			{
				ProcessSpace(reader, false);
				var lineNumberExpr = ParseNumericExpression(reader, lineNumber, false);
				if (lineNumberExpr == null)
				{
					break;
				}
				branches.Add(lineNumberExpr);

				ProcessSpace(reader, false);
				var comma = reader.Next(TokenType.Comma, false);
				if (comma == null)
				{
					break;
				}
				// There was a comma, so continue to the next line number.
			}

			if (branches.Count == 0)
			{
				throw new SyntaxException("EXPECTED A LINE NUMBER");
			}


			return new OnGotoStatement(value, branches);
		}
	}
}
