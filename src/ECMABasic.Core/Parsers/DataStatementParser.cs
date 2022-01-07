using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;
using System;
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

			var datums = new List<IExpression>();

			while (true)
			{
				ProcessSpace(reader, false);

				token = reader.Next(TokenType.String, false);
				if (token != null)
				{
					datums.Add(new StringExpression(token.Text.Substring(1, token.Text.Length - 2)));
				}
				else
				{
					var value = reader.NextNumber(false);
					if (value.HasValue)
					{
						datums.Add(new NumberExpression(value.Value));
					}
					else
					{
						// Read an unquoted string.

						token = null;
						var delimiters = new[] { TokenType.EndOfLine, TokenType.Comma };
						while (true)
						{
							var next = reader.Peek();
							if ((next == null) || (next.Type == TokenType.EndOfLine) || (next.Type == TokenType.Comma) || ((next.Type == TokenType.Symbol) && (next.Text == ",")))
							{
								break;
							} 

							if (token == null)
							{
								token = reader.Next();
							}
							else
							{
								token = new Token(TokenType.String, new[] { token, reader.Next() });
							}
						}

						if (token == null)
						{
							throw new SyntaxException("SYNTAX ERROR", lineNumber);
						}
						datums.Add(new StringExpression(token.Text.Trim()));
					}
				}

				ProcessSpace(reader, false);
				if (reader.Next(TokenType.Comma, false) == null)
				{
					break;
				}
			}

			return new DataStatement(datums);
		}
	}
}
