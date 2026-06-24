using ECMABasic.Application;
using ECMABasic.Domain;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace ECMABasic.Infrastructure;

public class TestEnvironment : EnvironmentBase
{
	private readonly StringBuilder _sb = new();
	private readonly Queue<string> _inputLines = new();

	public TestEnvironment(
		Interpreter? interpreter = null,
		IBasicConfiguration? config = null,
		ILogger? logger = null,
		string? input = null)
		: base(interpreter, config, logger)
	{
		if (input != null)
		{
			foreach (var line in input.Split('\n'))
			{
				AddInputLine(line);
			}
		}
	}

	public override int TerminalRow { get; set; } = 0;

	public override int TerminalColumn { get; set; } = 0;

	public string Text
	{
		get
		{
			return _sb.ToString();
		}
	}

	public string[] Lines
	{
		get
		{
			return Text.Split('\n');
		}
	}
	
	public void AddInputLine(string text)
	{
		_inputLines.Enqueue(text);
	}

	public override string ReadLine()
	{
		if (_inputLines.Count > 0)
		{
			return _inputLines.Dequeue();
		}
		return string.Empty;
	}

	public override void PrintLine(string text)
	{
		Print(text);
		_sb.AppendLine();
		Debug.WriteLine(string.Empty);
		TerminalRow++;
		TerminalColumn = 0;
	}

	public override void Print(string text)
	{
		_sb.Append(text);
		Debug.Write(text);
		TerminalColumn += text.Length;
	}

	public override void ReportError(string message)
	{
		// Traditional BASIC error reporting - print to console
		if (TerminalColumn != 0)
		{
			PrintLine(string.Empty);
		}
		PrintLine(message);

		// NEW: Structured logging if logger is available
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
		// There's no stopping the test environment.
	}
}
