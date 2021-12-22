using ECMABasic.Core.Configuration;
using ECMABasic.Core.Expressions;
using System;

namespace ECMABasic.Core.Parsers
{
	public class ListStatementParser : StatementParser
	{
		private readonly IBasicConfiguration _config;

		public ListStatementParser(IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
		}

		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "LIST");
			if (token == null)
			{
				return null;
			}

			if (ProcessSpace(reader, false) == null)
			{
				return new ListStatement(null, null);
			}

			var endToken = reader.Next(TokenType.Symbol, false, @"\-");
			if (endToken != null)
			{
				var onlyToExpr = ParseNumberExpression(reader, lineNumber, true) as NumberExpression;
				if (onlyToExpr.Value < 0)
				{
					throw new Exception("LINE NUMBER MUST BE > 0");
				}
				return new ListStatement(null, new IntegerExpression((int)onlyToExpr.Value));
			}

			var fromExpr = ParseNumberExpression(reader, lineNumber, false) as NumberExpression;
			if (fromExpr.Value < 0)
			{
				throw new Exception("LINE NUMBER MUST BE > 0");
			}

			endToken = reader.Next(TokenType.Symbol, false, @"\-");
			if (endToken == null)
			{
				return new ListStatement(new IntegerExpression((int)fromExpr.Value), null);
			}

			if (ParseNumberExpression(reader, lineNumber, false) is not NumberExpression toExpr)
			{
				toExpr = new NumberExpression(_config.MaxLineNumber);
			}
			else if (toExpr.Value < 0)
			{
				throw new Exception("LINE NUMBER MUST BE > 0");
			}

			return new ListStatement(new IntegerExpression((int)fromExpr.Value), new IntegerExpression((int)toExpr.Value));
		}
	}
}
