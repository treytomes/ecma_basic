using ECMABasic.Application;
using ECMABasic.Application.Parsers;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Direct tests of DefStatementParser to isolate the parsing bug.
/// </summary>
public class DefStatementParserTest
{
	[Fact]
	public void Parse_FND_SingleToken_Success()
	{
		// This is how "DEF FND(X)=..." gets tokenized
		var text = "DEF FND(X)=P*X/180\n";
		var reader = ComplexTokenReader.FromText(text);

		var parser = new DefStatementParser();
		var statement = parser.Parse(reader, 20);

		Assert.NotNull(statement);
	}

	[Fact]
	public void Parse_FN_D_TwoTokens_Success()
	{
		// This is how "DEF FN D(X)=..." gets tokenized
		var text = "DEF FN D(X)=P*X/180\n";
		var reader = ComplexTokenReader.FromText(text);

		var parser = new DefStatementParser();
		var statement = parser.Parse(reader, 20);

		Assert.NotNull(statement);
	}

	[Fact]
	public void Parse_FNA_SingleToken_Success()
	{
		var text = "DEF FNA=42\n";
		var reader = ComplexTokenReader.FromText(text);

		var parser = new DefStatementParser();
		var statement = parser.Parse(reader, 10);

		Assert.NotNull(statement);
	}
}
