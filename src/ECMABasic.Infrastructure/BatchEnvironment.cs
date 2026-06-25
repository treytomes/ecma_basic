using System;
using ECMABasic.Application;
using ECMABasic.Domain;
using Microsoft.Extensions.Logging;

namespace ECMABasic.Infrastructure;

/// <summary>
/// Batch (non-interactive) environment for ECMA-55 BASIC.
/// ECMA55-GEN-003: Support non-interactive execution.
/// Used when stdin/stdout are redirected or running from file.
/// </summary>
public class BatchEnvironment : EnvironmentBase
{
	private int _terminalRow = 0;
	private int _terminalColumn = 0;

	public BatchEnvironment(
		Interpreter? interpreter = null,
		IBasicConfiguration? config = null,
		ILogger? logger = null)
		: base(interpreter, config, logger)
	{
	}

	public override int TerminalRow
	{
		get => _terminalRow;
		set => _terminalRow = value;
	}

	public override int TerminalColumn
	{
		get => _terminalColumn;
		set => _terminalColumn = value;
	}

	public override string ReadLine()
	{
		// Read from stdin (redirected in batch mode)
		return Console.ReadLine() ?? string.Empty;
	}

	public override void PrintLine(string text)
	{
		Print(text);
		Console.WriteLine();
		_terminalRow++;
		_terminalColumn = 0;
	}

	public override void Print(string text)
	{
		Console.Write(text);
		_terminalColumn += text.Length;
	}

	public override void ReportError(string message)
	{
		// Traditional BASIC error reporting
		if (_terminalColumn != 0)
		{
			PrintLine(string.Empty);
		}
		PrintLine(message);

		// Structured logging if available
		if (Logger != null)
		{
			if (CurrentLineNumber > 0)
			{
				Logger.LogError("BASIC runtime error in line {LineNumber}: {ErrorMessage}", CurrentLineNumber, message);
			}
			else
			{
				Logger.LogError("BASIC runtime error: {ErrorMessage}", message);
			}
		}
	}

	public override void CheckForStopRequest()
	{
		// No-op: Cannot check keyboard in batch mode
		// Console.KeyAvailable throws when stdin is redirected
	}

	// Inherited PromptForInput() is no-op (correct for batch mode per ECMA55-DOC-014)
}
