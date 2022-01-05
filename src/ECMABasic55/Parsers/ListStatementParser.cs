using ECMABasic.Core;
using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using ECMABasic55.Statements;
using System;

namespace ECMABasic55.Parsers
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

			var expr = ParseNumericExpression(reader, lineNumber, true);

			if (expr is SubtractionExpression)
			{
				var from = (expr as SubtractionExpression).Left;
				var to = (expr as SubtractionExpression).Right;
				return new ListStatement(from, to);
			}
			else if (expr is NegationExpression)
			{
				var to = (expr as NegationExpression).Root;
				return new ListStatement(null, to);
			}
			else if (expr is NumberExpression)
			{
				var val = (expr as NumberExpression).Value;
				if (val < 0)
				{
					return new ListStatement(null, (expr as NumberExpression).Negate());
				}
				else
				{
					var from = expr;
					var minus = reader.Next(TokenType.Symbol, false, @"\-");
					if (minus != null)
					{
						var to = new NumberExpression(_config.MaxLineNumber);
						return new ListStatement(from, to);
					}
					else
					{
						return new ListStatement(from, null);
					}
				}
			}
			else
			{
				throw ExceptionFactory.LineNumberExpected();
			}
		}
	}
}
