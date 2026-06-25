using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Configuration;
using ECMABasic.Domain.Exceptions;
using ECMABasic.Application.Parsers;
using ECMABasic.Application.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Application;

// TODO: The end-goal is to have a single interpreter that you can plug feature sets into, like adding BASIC-1 on top of Minimal BASIC.

/// <summary>
/// Convert the source text into an abstract syntax tree.
/// </summary>
public class Interpreter
{
	private readonly List<StatementParser> _lineStatements;

	protected ComplexTokenReader _reader = null!;
	protected IEnvironment _environment = null!;
	private readonly IBasicConfiguration _config;

	/// <summary>
	/// Thread-local storage for the current parsing environment.
	/// Allows expression parsers to access the intrinsic registry during parsing.
	/// </summary>
	[ThreadStatic]
	private static IEnvironment? _currentParsingEnvironment;

	/// <summary>
	/// Gets the current parsing environment (thread-local).
	/// Used by expression parsers to access intrinsic registry.
	/// </summary>
	public static IEnvironment? CurrentParsingEnvironment => _currentParsingEnvironment;

	/// <summary>
	/// Sets the current parsing environment (thread-local).
	/// Protected so derived classes (like RuntimeInterpreter) can set it.
	/// </summary>
	protected static void SetCurrentParsingEnvironment(IEnvironment? env)
	{
		_currentParsingEnvironment = env;
	}

	public Interpreter(IBasicConfiguration? config = null)
	{
		_config = config ?? MinimalBasicConfiguration.Instance;

		_lineStatements = new List<StatementParser>()
		{
			new EndStatementParser(),
			new DefStatementParser(),
			new LetStatementParser(),
			new PrintStatementParser(),
			new StopStatementParser(),
			new RemarkStatementParser(),
			new GotoStatementParser(),
			new GosubStatementParser(),
			new ReturnStatementParser(),
			new IfThenStatementParser(),
			new OnGotoStatementParser(),
			new RestoreStatementParser(),
			new ReadStatementParser(),
			new DataStatementParser(),
			new InputStatementParser(),
			new NextStatementParser(),
			new RandomizeStatementParser(),
		};
	}

	/// <summary>
	/// Create an interpreter that will interpret the input text directly.
	/// </summary>
	/// <param name="text">The text to interpret.</param>
	/// <param name="env">The execution environment.</param>
	/// <param name="config">Optional BASIC configuration settings.</param>
	/// <returns>Was the input interpreted successfully?</returns>
	public static bool FromText(string text, IEnvironment env, IBasicConfiguration? config = null)
	{
		var interpreter = new Interpreter(config);
		return interpreter.InterpretProgramFromText(env, text);
	}

	/// <summary>
	/// Create an interpreter that will interpret the source text contained at the file path.
	/// </summary>
	/// <param name="path">The path to the file to interpret.</param>
	/// <param name="env">The execution environment.</param>
	/// <param name="config">Optional BASIC configuration settings.</param>
	/// <returns>Was the input interpreted successfully?</returns>
	public static bool FromFile(string path, IEnvironment env, IBasicConfiguration? config = null)
	{
		var interpreter = new Interpreter(config);
		return interpreter.InterpretProgramFromFile(env, path);
	}

	/// <summary>
	/// Allow interpretation of additional statements.
	/// </summary>
	/// <param name="additionalStatements">The statements to add to the interpreter.</param>
	public void InjectStatements(IEnumerable<StatementParser> additionalStatements)
	{
		_lineStatements.AddRange(additionalStatements);
	}

	/// <summary>
	/// Interpret the source text contained at the file path.
	/// </summary>
	/// <param name="env">The execution environment.</param>
	/// <param name="path">The path to the file to interpret.</param>
	/// <returns>Was the input interpreted successfully?</returns>
	public bool InterpretProgramFromFile(IEnvironment env, string path)
	{
		_reader = ComplexTokenReader.FromFile(path);
		return InterpretProgram(env);
	}

	/// <summary>
	/// Interpret the input text directly.
	/// </summary>
	/// <param name="env">The execution environment.</param>
	/// <param name="text">The text to interpret.</param>
	/// <returns>Was the input interpreted successfully?</returns>
	public bool InterpretProgramFromText(IEnvironment env, string text)
	{
		_reader = ComplexTokenReader.FromText(text);
		return InterpretProgram(env);
	}

	private bool InterpretProgram(IEnvironment env)
	{
		_environment = env;
		_currentParsingEnvironment = env;
		try
		{
			while (true)
			{
				if (!ProcessBlock(env, null!))
				{
					break;
				}
			}

			var token = _reader.Next();
			if (token != null)
			{
				throw new InvalidOperationException("EXPECTED END-OF-RECORD");
			}
			return true;
		}
		catch (SyntaxException e)
		{
			env.ReportError(e.Message);
			return false;
		}
		finally
		{
			_currentParsingEnvironment = null;
		}
	}

	/// <summary>
	/// Process the next line or for-block.
	/// </summary>
	/// <returns></returns>
	protected bool ProcessBlock(IEnvironment env, ProgramLine? parent)
	{
		return ProcessLine(env, parent) || ProcessForBlock(env, parent);
	}

	protected bool ProcessLine(IEnvironment env, ProgramLine? parent)
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

		// Default: reject standalone NEXT (batch loading mode)
		// RuntimeInterpreter overrides this for REPL incremental entry
		var statement = ProcessStatement(lineNumber, false, allowStandaloneNext: false);
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

	/// <summary>
	/// Read a line number off of the token stream.
	/// An exception will occur if a line number could not be read.
	/// </summary>
	/// <returns>The line number.</returns>
	protected int? ProcessLineNumber(bool throwsOnError = true)
	{
		var lineNumber = _reader.NextInteger(_config.MaxLineNumberDigits, throwsOnError);
		return lineNumber;
	}

	/// <summary>
	/// Read the end-of-line token off of the token stream.
	/// An exception will occur if the end-of-line could not be read.
	/// </summary>
	protected void ProcessEndOfLine()
	{
		var next = _reader.Next();
		if (next == null)
		{
			return;
		}
		if (next.Type != TokenType.EndOfLine)
		{
			throw new UnexpectedTokenException(TokenType.EndOfLine, next);
		}
	}

	/// <summary>
	/// Read whitespace off of the token stream.
	/// An exception will optionally occur if whitespace could not be found.
	/// </summary>
	/// <param name="throwOnError">Throw an exception if space is not found.  Default to true.</param>
	/// <returns>The space token.</returns>
	protected Token? ProcessSpace(bool throwOnError = true)
	{
		return _reader.Next(TokenType.Space, throwOnError);
	}

	protected IStatement? ProcessStatement(int? lineNumber, bool throwOnError = true, bool allowStandaloneNext = false)
	{
		// In REPL mode (allowStandaloneNext=true), also try to parse FOR statements
		// In batch mode, FOR statements are parsed by ProcessForBlock
		if (allowStandaloneNext)
		{
			var forStmt = new ForStatementParser().Parse(_reader, lineNumber);
			if (forStmt != null)
			{
				return forStmt;
			}
		}

		foreach (var parser in _lineStatements)
		{
			var stmt = parser.Parse(_reader, lineNumber);
			if (stmt != null)
			{
				// In batch/file loading mode, reject standalone NEXT statements
				// In REPL mode (allowStandaloneNext=true), allow incremental entry
				if (stmt is NextStatement && !allowStandaloneNext)
				{
					throw ExceptionFactory.NextWithoutFor(lineNumber);
				}
				return stmt;
			}
		}

		if (throwOnError)
		{
			throw new SyntaxException("A STATEMENT WAS EXPECTED", lineNumber);
		}
		return null;
	}

	private void ValidateNotUsingPreviousControlVariable(ProgramLine forLine, ProgramLine? parent)
	{
		if (parent == null)
		{
			return;
		}

		if (parent.Statement is not ForStatement stmt)
		{
			return;
		}

		if (stmt.LoopVar.Name == (forLine.Statement as ForStatement)!.LoopVar.Name)
		{
			throw ExceptionFactory.ForUsingPreviousControlVariable(forLine.LineNumber);
		}

		if (parent.Parent == null)
		{
			return;
		}

		ValidateNotUsingPreviousControlVariable(forLine, parent.Parent);
	}

	private bool ProcessForBlock(IEnvironment env, ProgramLine? parent)
	{
		var program = ((EnvironmentBase)env).Program;
		if (!ProcessForLine(env, parent))
		{
			return false;
		}
		var forLine = program.Last();
		ValidateNotUsingPreviousControlVariable(forLine, parent);

		while (true)
		{
			if (ProcessNextLine(env, forLine))
			{
				break;
			}
			if (!ProcessBlock(env, forLine))
			{
				break;
			}
		}

		if (!(program.Last().Statement is NextStatement))
		{
			throw ExceptionFactory.ForWithoutNext(forLine.LineNumber);
		}

		return true;
	}

	protected bool ProcessForLine(IEnvironment env, ProgramLine? parent)
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
			program.Delete(lineNumber.Value);
			return true;
		}

		var statement = new ForStatementParser().Parse(_reader, lineNumber);
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

	protected bool ProcessNextLine(IEnvironment env, ProgramLine? parent)
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

		var statement = new NextStatementParser().Parse(_reader, lineNumber);
		if (statement == null)
		{
			_reader.Seek(startIndex);
			return false;
		}

		if ((statement as NextStatement)!.LoopVar.Name != (parent!.Statement as ForStatement)!.LoopVar.Name)
		{
			throw ExceptionFactory.NextWithoutFor(lineNumber);
		}

		// Optional space.
		ProcessSpace(false);

		// Require an end-of-line.
		ProcessEndOfLine();

		program.Insert(new ProgramLine(lineNumber.Value, statement, parent));
		return true;
	}
}
