using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;
using System.Collections.Generic;

namespace ECMABasic.Core
{
	public class PrintStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "PRINT");
			if (token == null)
			{
				return null;
			}

			var spaceToken = ProcessSpace(reader, false);
			if (spaceToken == null)
			{
				// It's just an empty print statement.
				return new PrintStatement();
			}

			var printList = ProcessPrintList(reader, lineNumber);
			return new PrintStatement(printList);
		}

		private static List<IPrintItem> ProcessPrintList(ComplexTokenReader reader, int? lineNumber = null)
		{
			var items = new List<IPrintItem>();

			while (true)
			{
				var printItem = ProcessPrintItem(reader, lineNumber);
				if (printItem != null)
				{
					items.Add(printItem);
				}

				ProcessSpace(reader, false);  // Optional space.

				var printSeparator = ProcessPrintSeparator(reader);

				ProcessSpace(reader, false);  // Optional space.

				if (printSeparator == null)
				{
					// No separator, so this is the end of the list.
					return items;
				}
				else
				{
					// If there's a separator, there might be another item.
					items.Add(printSeparator);
				}
			}
		}

		private static IPrintItemSeparator ProcessPrintSeparator(ComplexTokenReader reader)
		{
			var symbolToken = reader.Next(TokenType.Comma, false);
			if (symbolToken != null)
			{
				return new CommaExpression();
			}

			symbolToken = reader.Next(TokenType.Semicolon, false);
			if (symbolToken != null)
			{
				return new SemicolonExpression();
			}

			return null;
		}

		private static IPrintItem ProcessPrintItem(ComplexTokenReader reader, int? lineNumber = null)
		{
			var expr = ProcessTabExpression(reader, lineNumber);
			if (expr != null)
			{
				return expr;
			}

			expr = ParseExpression(reader, lineNumber, false);
			if (expr != null)
			{
				return expr;
			}

			return null;
		}

		private static IPrintItem ProcessTabExpression(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "TAB");
			if (token == null)
			{
				return null;
			}

			reader.Next(TokenType.OpenParenthesis);

			var valueExpr = ParseNumericExpression(reader, lineNumber, true);

			reader.Next(TokenType.CloseParenthesis);

			return new TabExpression(valueExpr);
		}
	}
}
