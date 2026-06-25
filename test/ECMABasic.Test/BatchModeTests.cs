using System;
using System.IO;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for batch (non-interactive) execution mode per ECMA-55.
/// ECMA55-GEN-003: Support non-interactive execution.
/// ECMA55-INP-003: INPUT reads from stdin without prompts in batch mode.
/// ECMA55-DOC-014: Suppress prompts in non-interactive mode.
/// </summary>
public class BatchModeTests
{
	#region BatchEnvironment Basic Behavior

	[Fact]
	public void BatchEnvironment_CheckForStopRequest_DoesNotThrow()
	{
		// In batch mode, CheckForStopRequest should not try to access keyboard
		var env = new BatchEnvironment();

		// Should not throw (ConsoleEnvironment would throw when stdin redirected)
		var exception = Record.Exception(() => env.CheckForStopRequest());
		Assert.Null(exception);
	}

	[Fact]
	public void BatchEnvironment_TerminalPosition_TracksInternally()
	{
		// Batch mode can't access Console.CursorTop/Left, must track internally
		var env = new BatchEnvironment();

		Assert.Equal(0, env.TerminalRow);
		Assert.Equal(0, env.TerminalColumn);

		env.TerminalRow = 5;
		env.TerminalColumn = 10;

		Assert.Equal(5, env.TerminalRow);
		Assert.Equal(10, env.TerminalColumn);
	}

	[Fact]
	public void BatchEnvironment_Print_UpdatesColumn()
	{
		var env = new BatchEnvironment();
		var output = new StringWriter();
		Console.SetOut(output);

		env.Print("HELLO");

		Assert.Equal(5, env.TerminalColumn);
		Assert.Equal("HELLO", output.ToString());
	}

	[Fact]
	public void BatchEnvironment_PrintLine_ResetsColumn()
	{
		var env = new BatchEnvironment();
		var output = new StringWriter();
		Console.SetOut(output);

		env.Print("HELLO");
		Assert.Equal(5, env.TerminalColumn);

		env.PrintLine(" WORLD");
		Assert.Equal(0, env.TerminalColumn);
		Assert.Equal(1, env.TerminalRow);
	}

	#endregion

	#region INPUT Statement in Batch Mode

	[Fact]
	public void BatchEnvironment_InputStatement_NoPrompt()
	{
		// ECMA55-DOC-014: INPUT should not print "? " in batch mode
		var input = new StringReader("42\n");
		var output = new StringWriter();
		Console.SetIn(input);
		Console.SetOut(output);

		var env = new BatchEnvironment();
		var program = "10 INPUT A\n20 PRINT A\n30 END\n";

		Interpreter.FromText(program, env);
		env.Program.Execute(env);

		var result = output.ToString();

		// Should NOT contain the "? " prompt
		Assert.DoesNotContain("?", result);
		// Should contain the printed value
		Assert.Contains("42", result);
	}

	[Fact]
	public void ConsoleEnvironment_InputStatement_HasPrompt()
	{
		// Verify ConsoleEnvironment DOES print prompt (for comparison)
		var input = new StringReader("42\n");
		var output = new StringWriter();
		Console.SetIn(input);
		Console.SetOut(output);

		var env = new ConsoleEnvironment();
		var program = "10 INPUT A\n20 PRINT A\n30 END\n";

		Interpreter.FromText(program, env);
		env.Program.Execute(env);

		var result = output.ToString();

		// Should contain the "? " prompt
		Assert.Contains("?", result);
	}

	#endregion

	#region Program Execution with File

	[Fact]
	public void BatchMode_ExecutesProgramFromFile()
	{
		// Create a test program
		var tempFile = Path.GetTempFileName();
		File.WriteAllText(tempFile, "10 PRINT \"BATCH MODE\"\n20 END\n");

		try
		{
			var output = new StringWriter();
			Console.SetOut(output);

			var env = new BatchEnvironment();
			env.LoadFile(tempFile);
			env.Program.Execute(env);

			var result = output.ToString();
			Assert.Contains("BATCH MODE", result);
		}
		finally
		{
			File.Delete(tempFile);
		}
	}

	[Fact]
	public void BatchMode_HandlesInputFromStdin()
	{
		// Program reads two numbers and prints their sum
		var program = @"10 INPUT A
20 INPUT B
30 PRINT A + B
40 END
";

		var input = new StringReader("10\n20\n");
		var output = new StringWriter();
		Console.SetIn(input);
		Console.SetOut(output);

		var env = new BatchEnvironment();
		Interpreter.FromText(program, env);
		env.Program.Execute(env);

		var result = output.ToString();

		// Should not have prompts, just the result
		Assert.DoesNotContain("?", result);
		Assert.Contains("30", result);
	}

	#endregion

	#region Error Handling in Batch Mode

	[Fact]
	public void BatchMode_ReportsErrors()
	{
		// Use a program that causes a runtime error
		var program = "10 INPUT A\n20 END\n"; // Will fail with OUT OF INPUT when stdin is empty

		var input = new StringReader(""); // Empty input
		var output = new StringWriter();
		Console.SetIn(input);
		Console.SetOut(output);

		var env = new BatchEnvironment();
		Interpreter.FromText(program, env);

		// Execute should handle error gracefully
		var exception = Record.Exception(() => env.Program.Execute(env));

		// Error should be reported via ReportError, not thrown
		Assert.Null(exception);

		var result = output.ToString();
		Assert.Contains("OUT OF INPUT", result);
	}

	#endregion

	#region Comparison: Interactive vs Batch Output

	[Fact]
	public void SameProgram_DifferentModes_SameOutputExceptPrompts()
	{
		var program = @"10 LET A = 5
20 LET B = 10
30 PRINT A + B
40 END
";

		// Run in batch mode
		var batchOutput = new StringWriter();
		Console.SetOut(batchOutput);
		var batchEnv = new BatchEnvironment();
		Interpreter.FromText(program, batchEnv);
		batchEnv.Program.Execute(batchEnv);

		// Run in console mode (no input needed)
		var consoleOutput = new StringWriter();
		Console.SetOut(consoleOutput);
		var consoleEnv = new ConsoleEnvironment();
		Interpreter.FromText(program, consoleEnv);
		consoleEnv.Program.Execute(consoleEnv);

		// Both should produce "15"
		Assert.Contains("15", batchOutput.ToString());
		Assert.Contains("15", consoleOutput.ToString());
	}

	#endregion
}
