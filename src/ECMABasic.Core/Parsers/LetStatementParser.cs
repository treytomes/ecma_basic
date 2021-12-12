using ECMABasic.Core.Expressions;
using ECMABasic.Core.Statements;

namespace ECMABasic.Core
{
	public class LetStatementParser : StatementParser
	{
		public override IStatement Parse(ComplexTokenReader reader, int? lineNumber = null)
		{
			var token = reader.Next(TokenType.Word, false, "LET");
			if (token == null)
			{
				return null;
			}

			ProcessSpace(reader, true);

			VariableExpression targetExpr;
			var variableNameToken = reader.Next(TokenType.StringVariable, false);
			if (variableNameToken != null)
			{
				targetExpr = new VariableExpression(variableNameToken.Text);
			}
			else
			{
				variableNameToken = reader.Next(TokenType.NumericVariable);
				targetExpr = new VariableExpression(variableNameToken.Text);
			}

			ProcessSpace(reader, false);

			reader.Next(TokenType.Equals);

			ProcessSpace(reader, false);

			Token valueToken;
			IExpression valueExpr;
			if (targetExpr.IsString)
			{
				valueToken = reader.Next(TokenType.String, false);
				if (valueToken != null)
				{
					// The actual string is everything between the "".
					var text = valueToken.Text[1..^1];
					valueExpr = new StringExpression(text);
				}
				else
				{
					valueToken = reader.Next(TokenType.StringVariable);
					valueExpr = new VariableExpression(valueToken.Text);
				}
			}
			else
			{
				var isNegative = reader.Next(TokenType.Negation, false) != null;

				var number = reader.NextNumber(false);
				if (number != null)
				{
					if (isNegative)
					{
						number = -number.Value;
					}
					valueExpr = new NumberExpression(number.Value);
				}
				else
				{
					valueToken = reader.Next(TokenType.NumericVariable);
					valueExpr = new VariableExpression(valueToken.Text);
				}
			}

			return new LetStatement(targetExpr, valueExpr);
		}
	}
}
