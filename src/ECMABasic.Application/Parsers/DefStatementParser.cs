using ECMABasic.Domain;
using ECMABasic.Domain.Exceptions;
using ECMABasic.Application.Statements;

namespace ECMABasic.Application;

/// <summary>
/// Parses DEF FN statement per ECMA-55 Section 10.
/// ECMA55-DEF-001: DEF FNx = expression or DEF FNx(parameter) = expression
/// </summary>
public class DefStatementParser : StatementParser
{
	public override IStatement? Parse(ComplexTokenReader reader, int? lineNumber = null)
	{
		var defToken = reader.Next(TokenType.Word, false, "DEF");
		if (defToken == null)
		{
			return null;
		}

		// Optional space after DEF
		reader.Next(TokenType.Space, false);

		// Expect FN followed by single letter
		// Could be either "FN A" (two tokens) or "FNA" (one token)
		var nameToken = reader.Next(TokenType.Word, false);
		if (nameToken == null)
		{
			throw new SyntaxException("Expected function name after DEF", lineNumber);
		}

		string functionName;
		if (nameToken.Text.ToUpper() == "FN")
		{
			// "DEF FN A" form - FN and letter are separate
			var letterToken = reader.Next(TokenType.Word, false);
			if (letterToken == null || letterToken.Text.Length != 1 || !char.IsLetter(letterToken.Text[0]))
			{
				throw new SyntaxException("Function name must be FN followed by a single letter (A-Z)", lineNumber);
			}
			functionName = "FN" + letterToken.Text.ToUpper();
		}
		else if (nameToken.Text.Length == 3 &&
		         nameToken.Text.Substring(0, 2).ToUpper() == "FN" &&
		         char.IsLetter(nameToken.Text[2]))
		{
			// "DEF FNA" form - FNA is a single token
			functionName = nameToken.Text.ToUpper();
		}
		else
		{
			throw new SyntaxException("Function name must be FN followed by a single letter (A-Z)", lineNumber);
		}

		// Check for optional parameter: (X)
		string? parameter = null;
		if (reader.Next(TokenType.OpenParenthesis, false) != null)
		{
			var paramToken = reader.Next(TokenType.Word, false);
			if (paramToken == null)
			{
				throw new SyntaxException("Expected parameter name", lineNumber);
			}
			parameter = paramToken.Text;

			reader.Next(TokenType.CloseParenthesis);
		}

		// Expect =
		reader.Next(TokenType.Space, false);
		reader.Next(TokenType.Symbol, true, "=");
		reader.Next(TokenType.Space, false);

		// Parse the function body expression
		var bodyExpr = new NumericExpressionParser(reader, lineNumber, true).Parse();
		if (bodyExpr == null)
		{
			throw new SyntaxException("Expected expression after =", lineNumber);
		}

		return new DefStatement(functionName, parameter, bodyExpr);
	}
}
