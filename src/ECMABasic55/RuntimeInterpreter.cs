using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Configuration;
using ECMABasic.Domain.Exceptions;
using ECMABasic55.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic55;

/// <summary>
/// The core interpreter with immediate-mode statements tacked on.
/// </summary>
public class RuntimeInterpreter : Interpreter
{
	private readonly List<StatementParser> _immediateStatements;

	public RuntimeInterpreter(IBasicConfiguration? config = null)
		: base(config)
	{
		_immediateStatements = new List<StatementParser>()
		{
			new RunStatementParser(),
			new NewStatementParser(),
			new ContinueStatementParser(),
			new LoadStatementParser(),
			new ListStatementParser(),
			new SaveStatementParser(),
		};
	}

	public IStatement? ProcessImmediate(IEnvironment env, string text)
	{
		try
		{
			_reader = ComplexTokenReader.FromText(text);

			// Use ProcessLineForREPL instead of ProcessBlock to allow incremental entry
			// ProcessBlock expects complete FOR-NEXT blocks, which breaks REPL UX
			if (ProcessLineForREPL(env, null))
			{
				return null;
			}
			else
			{
				ProcessSpace(false);
				var statement = ProcessStatement(null, false);
				if (statement == null)
				{
					statement = ProcessImmediateStatement();
				}

				ProcessSpace(false);
				ProcessEndOfLine();
				return statement;
			}
		}
		catch (SyntaxException)
		{
			throw;
		}
		catch (Exception)
		{
			throw ECMABasic.Domain.ExceptionFactory.Syntax();
		}
	}

	/// <summary>
	/// Process a program line in REPL mode, allowing standalone NEXT statements
	/// for incremental FOR loop entry.
	/// </summary>
	private bool ProcessLineForREPL(IEnvironment env, ProgramLine? parent)
	{
		var program = ((EnvironmentBase)env).Program;
		var startIndex = _reader.TokenIndex;
		if (_reader.IsAtEnd)
		{
			return false;
		}

		var lineNumber = ProcessLineNumber(false);
		if (!lineNumber.HasValue)
		{
			_reader.Seek(startIndex);
			return false;
		}

		if (ProcessSpace(false) == null)
		{
			// An empty line triggers a deletion.
			program.Insert(new ProgramLine(lineNumber.Value, null, null));
			return true;
		}

		// Allow standalone NEXT and FOR for incremental REPL entry
		var statement = ProcessStatement(lineNumber, false, allowStandaloneNext: true);
		if (statement == null)
		{
			_reader.Seek(startIndex);
			return false;
		}

		// Optional space.
		ProcessSpace(false);

		// Require an end-of-line.
		ProcessEndOfLine();

		program.Insert(new ProgramLine(lineNumber.Value, statement, parent));
		return true;
	}

	private IStatement? ProcessImmediateStatement()
	{
		foreach (var parser in _immediateStatements)
		{
			var stmt = parser.Parse(_reader);
			if (stmt != null)
			{
				return stmt;
			}
		}
		return null;
	}
}
