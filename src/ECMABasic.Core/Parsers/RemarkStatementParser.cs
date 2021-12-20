using ECMABasic.Core.Statements;
using System.Text;

namespace ECMABasic.Core.Parsers
{
	public class RemarkStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "REM");
			if (token == null)
			{
				return null;
			}

			if (ProcessSpace(reader, false) == null)
			{
				return new RemarkStatement(string.Empty);
			}

			var sb = new StringBuilder();
			while (true)
			{
				if (reader.Peek().Type == TokenType.EndOfLine)
				{
					break;
				}

				// TODO: The tokenizer is getting choked up if it finds a double-quote in the text.
				sb.Append(reader.Next().Text);
			}

			return new RemarkStatement(sb.ToString());
		}
	}
}
