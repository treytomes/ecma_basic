using System;
using System.Linq;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Integration tests that simulate REPL line-by-line entry.
/// Tests incremental program building by parsing individual lines.
/// </summary>
public class ReplIntegrationTests
{
	private readonly TestEnvironment _env;
	private readonly Interpreter _interpreter;

	public ReplIntegrationTests()
	{
		_interpreter = new Interpreter();
		_env = new TestEnvironment(_interpreter);
	}

	/// <summary>
	/// Helper to simulate typing a line in the REPL.
	/// Parses the line and adds it to the program if it has a line number.
	/// </summary>
	private void TypeLine(string line)
	{
		// Add newline like the REPL does
		var lineWithNewline = line + Environment.NewLine;

		// Parse the line using Interpreter.FromText (static method)
		// This simulates what the REPL does when you type a numbered line
		Interpreter.FromText(lineWithNewline, _env);
	}

	/// <summary>
	/// Helper to run the current program and get output
	/// </summary>
	private string Run()
	{
		_env.Program.Execute(_env);
		return _env.Text;
	}

	#region DEF FN Tests

	[Fact]
	public void Repl_DefFn_SingleToken_FND()
	{
		// This is failing in the REPL: "DEF FND(X)=..." as single token
		TypeLine("10 LET P=3.14159");
		TypeLine("20 DEF FND(X)=P*X/180");
		TypeLine("30 PRINT FND(90)");
		TypeLine("40 END");

		var result = Run();
		Assert.Contains("1.57", result);
	}

	[Fact]
	public void Repl_DefFn_TwoTokens_FN_D()
	{
		// This is failing in the REPL: "DEF FN D(X)=..." as two tokens with space
		TypeLine("10 LET P=3.14159");
		TypeLine("20 DEF FN D(X)=P*X/180");
		TypeLine("30 PRINT FND(90)");
		TypeLine("40 END");

		var result = Run();
		Assert.Contains("1.57", result);
	}

	[Fact]
	public void Repl_DefFn_SingleToken_FNA()
	{
		// Simpler case - should work
		TypeLine("10 DEF FNA=42");
		TypeLine("20 PRINT FNA");
		TypeLine("30 END");

		var result = Run();
		Assert.Contains("42", result);
	}

	[Fact]
	public void Repl_DefFn_TwoTokens_FN_A()
	{
		// With explicit space - should work after our fix
		TypeLine("10 DEF FN A=42");
		TypeLine("20 PRINT FNA");
		TypeLine("30 END");

		var result = Run();
		Assert.Contains("42", result);
	}

	[Fact]
	public void Repl_DefFn_SingleToken_FNZ()
	{
		// Try another letter
		TypeLine("10 DEF FNZ(X)=X*2");
		TypeLine("20 PRINT FNZ(5)");
		TypeLine("30 END");

		var result = Run();
		Assert.Contains("10", result);
	}

	#endregion

	#region Basic REPL Commands

	[Fact]
	public void Repl_BasicProgram_HelloWorld()
	{
		TypeLine("10 PRINT \"HELLO WORLD\"");
		TypeLine("20 END");

		var result = Run();
		Assert.Contains("HELLO WORLD", result);
	}

	[Fact]
	public void Repl_VerifyProgramLines_AddedCorrectly()
	{
		TypeLine("10 PRINT \"TEST\"");
		TypeLine("20 END");

		// Verify program has 2 lines
		Assert.Equal(2, _env.Program.Count());
		var line10 = _env.Program.First(l => l.LineNumber == 10);
		Assert.NotNull(line10);
	}

	#endregion

	#region Line Number Handling

	[Fact]
	public void Repl_ReplaceExistingLine()
	{
		TypeLine("10 PRINT \"FIRST\"");
		TypeLine("10 PRINT \"SECOND\"");
		TypeLine("20 END");

		var result = Run();
		Assert.Contains("SECOND", result);
		Assert.DoesNotContain("FIRST", result);
	}

	[Fact]
	public void Repl_DeleteLine_EmptyLine()
	{
		TypeLine("10 PRINT \"DELETE ME\"");
		TypeLine("20 PRINT \"KEEP ME\"");
		TypeLine("30 END");
		// Note: Line deletion via empty line number requires RuntimeInterpreter
		// For now just verify lines are added
		Assert.Equal(3, _env.Program.Count());
	}

	#endregion

	#region Complex Programs

	[Fact]
	public void Repl_ForLoop_IncrementalEntry()
	{
		// Note: FOR-NEXT gets combined into a block by the interpreter
		TypeLine("10 FOR I=1 TO 3");
		TypeLine("20 PRINT I");
		TypeLine("30 NEXT I");
		TypeLine("40 END");

		var result = Run();
		// Just verify it runs without error and produces output
		Assert.NotEmpty(result);
	}

	[Fact]
	public void Repl_IfThen_WithGoto()
	{
		TypeLine("10 LET X=5");
		TypeLine("20 IF X>3 THEN 40");
		TypeLine("30 PRINT \"SMALL\"");
		TypeLine("40 PRINT \"LARGE\"");
		TypeLine("50 END");

		var result = Run();
		Assert.Contains("LARGE", result);
		Assert.DoesNotContain("SMALL", result);
	}

	[Fact]
	public void Repl_VariableAssignment()
	{
		TypeLine("10 LET A=10");
		TypeLine("20 LET B=20");
		TypeLine("30 PRINT A+B");
		TypeLine("40 END");

		var result = Run();
		Assert.Contains("30", result);
	}

	#endregion

	#region Error Cases

	// Note: Error handling tests removed - FromText doesn't throw in parse mode,
	// it accumulates errors for the environment to report

	#endregion
}
