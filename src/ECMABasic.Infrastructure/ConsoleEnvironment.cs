using ECMABasic.Application;
using ECMABasic.Domain;
using Microsoft.Extensions.Logging;

namespace ECMABasic.Infrastructure;

public class ConsoleEnvironment : EnvironmentBase
{
	public ConsoleEnvironment(
		Interpreter? interpreter = null,
		IBasicConfiguration? config = null,
		ILogger<ConsoleEnvironment>? logger = null)
		: base(interpreter, config, logger)
	{
	}

	public override int TerminalRow
	{
		get
		{
			return Console.CursorTop;
		}
		set
		{
			Console.CursorTop = value;
		}
	}

	public override int TerminalColumn
	{
		get
		{
			return Console.CursorLeft;
		}
		set
		{
			Console.CursorLeft = value;
		}
	}

	public override string ReadLine()
	{
		return Console.ReadLine() ?? string.Empty;
	}

	public override void PrintLine(string text)
	{
		Print(text);
		Console.WriteLine();
	}

	public override void Print(string text)
	{
		Console.Write(text);
	}

	public override void ReportError(string message)
	{
		// Traditional BASIC error reporting - print to console
		PrintLine(string.Empty);
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
		if (Console.KeyAvailable)
		{
			var key = Console.ReadKey(true);
			if (key.Key == ConsoleKey.Escape)
			{
				throw Domain.ExceptionFactory.ProgramStop(CurrentLineNumber);
			}
		}
	}
}
