using ECMABasic.Application;
using ECMABasic.Application.Parsers;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Direct tests of DefStatementParser to isolate the parsing bug.
/// </summary>
public class DefStatementParserTest
{
	private readonly Interpreter _interpreter;
	private readonly TestEnvironment _env;

	public DefStatementParserTest()
	{
		_interpreter = new Interpreter();
		_env = new TestEnvironment(_interpreter);
	}
	[Fact]
	public void Parse_FND_SingleToken_Success()
	{
		// This is how "DEF FND(X)=..." gets tokenized
		var text = "10 DEF FND(X)=P*X/180\n20 END\n";

		// Parse and execute to register function
		var parseSuccess = _interpreter.InterpretProgramFromText(_env, text);
		Assert.True(parseSuccess, "Program should parse successfully");

		_env.Program.Execute(_env);

		Assert.True(_env.Functions.IsDefined("FND"), "Function FND should be defined");
	}

	[Fact]
	public void Parse_FN_D_TwoTokens_Success()
	{
		// This is how "DEF FN D(X)=..." gets tokenized
		var text = "10 DEF FN D(X)=P*X/180\n20 END\n";

		// Parse and execute to register function
		var parseSuccess = _interpreter.InterpretProgramFromText(_env, text);
		Assert.True(parseSuccess, "Program should parse successfully");

		_env.Program.Execute(_env);

		Assert.True(_env.Functions.IsDefined("FND"), "Function FND should be defined");
	}

	[Fact]
	public void Parse_FNA_SingleToken_Success()
	{
		var text = "10 DEF FNA=42\n20 END\n";

		// Parse and execute to register function
		var parseSuccess = _interpreter.InterpretProgramFromText(_env, text);
		Assert.True(parseSuccess, "Program should parse successfully");

		_env.Program.Execute(_env);

		Assert.True(_env.Functions.IsDefined("FNA"), "Function FNA should be defined");
	}
}
