using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Configuration;
using ECMABasic.Application.Exceptions;
using System;

namespace ECMABasic55;

public class ConsoleEnvironment : EnvironmentBase
{
	public ConsoleEnvironment(Interpreter? interpreter = null, IBasicConfiguration? config = null)
		: base(interpreter, config)
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
		PrintLine(string.Empty);
		PrintLine(message);
	}

	public override void CheckForStopRequest()
	{
		if (Console.KeyAvailable)
		{
			var key = Console.ReadKey(true);
			if (key.Key == ConsoleKey.Escape)
			{
				throw ECMABasic.Application.ExceptionFactory.ProgramStop(CurrentLineNumber);
			}
		}
	}
}
