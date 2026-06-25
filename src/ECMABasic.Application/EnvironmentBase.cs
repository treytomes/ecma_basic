using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application.Configuration;
using ECMABasic.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Application;

public abstract class EnvironmentBase : IEnvironment
{
	private readonly Dictionary<string, string> _stringVariables = new();
	private readonly Dictionary<string, double> _numericVariables = new();
	private readonly Stack<ICallStackContext> _callStack = new();
	private readonly DataPointer _dataPointer = new();
	private readonly Stack<Dictionary<string, double>> _scopeStack = new();

	public EnvironmentBase(Interpreter? interpreter = null, IBasicConfiguration? config = null, ILogger? logger = null)
	{
		Interpreter = interpreter ?? new Interpreter();
		Configuration = config ?? MinimalBasicConfiguration.Instance;
		Logger = logger;
		Program = new Program();
		Intrinsics = new IntrinsicRegistry();
		Functions = new FunctionRegistry();
		Random = new BasicRandomNumberGenerator();
	}

	public IBasicConfiguration Configuration { get; }

	public IIntrinsicRegistry Intrinsics { get; }

	public IFunctionRegistry Functions { get; }

	public IRandomNumberGenerator Random { get; }

	public ILogger? Logger { get; }

	public Interpreter Interpreter { get; }

	/// <summary>
	/// The line number currently being executed.
	/// </summary>
	public int CurrentLineNumber { get; set; }

	public abstract int TerminalRow { get; set; }

	public abstract int TerminalColumn { get; set; }

	public Program Program { get; private set; }

	public abstract string ReadLine();

	public abstract void Print(string text);

	public abstract void PrintLine(string text = "");

	public abstract void ReportError(string message);

	/// <summary>
	/// Prompt for INPUT statement. Default is no prompt (batch mode).
	/// Override in ConsoleEnvironment to print "? ".
	/// </summary>
	public virtual void PromptForInput()
	{
		// Default: no prompt (batch mode per ECMA55-DOC-014)
	}

	public string GetStringVariableValue(string variableName)
	{
		// TODO: Validate variable name?
		if (!_stringVariables.ContainsKey(variableName))
		{
			var value = string.Empty;
			SetStringVariableValue(variableName, value);
			return value;
		}
		return _stringVariables[variableName];
	}
	
	public void SetStringVariableValue(string variableName, string value)
	{
		// TODO: Validate variable name?
		if (value.Length > Configuration.MaxStringLength)
		{
			throw ExceptionFactory.StringOverflow(CurrentLineNumber);
		}
		if (!variableName.EndsWith("$"))
		{
			throw ExceptionFactory.MixedStringsAndNumbers(CurrentLineNumber);
		}
		_stringVariables[variableName] = value;
	}

	public double GetNumericVariableValue(string variableName)
	{
		// TODO: Validate variable name?

		// Check scope stack first (for function parameters)
		// ECMA55-DEF-002: Function parameter shadows global variables
		if (_scopeStack.Count > 0)
		{
			var currentScope = _scopeStack.Peek();
			if (currentScope.ContainsKey(variableName))
			{
				return currentScope[variableName];
			}
		}

		// Fall back to global variables
		if (!_numericVariables.ContainsKey(variableName))
		{
			var value = 0;
			SetNumericVariableValue(variableName, value);
			return value;
		}
		return _numericVariables[variableName];
	}

	public void SetNumericVariableValue(string variableName, double value)
	{
		// TODO: Validate variable name?
		if (variableName.EndsWith("$"))
		{
			throw ExceptionFactory.MixedStringsAndNumbers(CurrentLineNumber);
		}
		_numericVariables[variableName] = value;
	}

	public void Clear()
	{
		Program.Clear();
		_numericVariables.Clear();
		_stringVariables.Clear();
		CurrentLineNumber = 0;
	}

	public bool ValidateLineNumber(int lineNumber, bool throwsIfMissing = false)
	{
		if (!Program.Any(x => x.LineNumber == lineNumber))
		{
			if (throwsIfMissing)
			{
				throw ExceptionFactory.UndefinedLineNumber(lineNumber, CurrentLineNumber);
			}
			else
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}

	public void PushCallStack(ICallStackContext context)
	{
		_callStack.Push(context);
	}

	public ICallStackContext? PopCallStack()
	{
		if (_callStack.Count == 0)
		{
			return null;
		}
		return _callStack.Pop();
	}

	public abstract void CheckForStopRequest();

	public bool LoadFile(string filename)
	{
		Program.Clear();
		return Interpreter.InterpretProgramFromFile(this, filename);
	}

	public IExpression ReadData()
	{
		var data = Program.GetDataLine(_dataPointer.LineIndex);
		if (data == null)
		{
			throw ExceptionFactory.OutOfData(CurrentLineNumber);
		}
		var datum = data.Datums[_dataPointer.DatumIndex];
		_dataPointer.DatumIndex++;
		if (_dataPointer.DatumIndex >= data.Datums.Count)
		{
			_dataPointer.DatumIndex = 0;
			_dataPointer.LineIndex++;
		}
		return datum;
	}

	public void ResetDataPointer()
	{
		_dataPointer.LineIndex = 0;
		_dataPointer.DatumIndex = 0;
	}

	public int GetNextLineNumber(int fromLineNumber)
	{
		return Program.GetNextLineNumber(fromLineNumber);
	}

	public IStatement? MoveToNextLine()
	{
		var programLine = Program.MoveToNextLine(this);
		return programLine?.Statement;
	}

	public IStatement? GetStatementAtLine(int lineNumber)
	{
		var programLine = Program[lineNumber];
		return programLine?.Statement;
	}

	/// <summary>
	/// Push a new scope onto the scope stack for function parameter evaluation.
	/// ECMA55-DEF-002: Function parameters shadow global variables with same name.
	/// </summary>
	/// <param name="paramName">Parameter variable name</param>
	/// <param name="value">Parameter value</param>
	public void PushScope(string paramName, double value)
	{
		var scope = new Dictionary<string, double>
		{
			[paramName] = value
		};
		_scopeStack.Push(scope);
	}

	/// <summary>
	/// Pop the current scope from the scope stack.
	/// Called after function evaluation completes.
	/// </summary>
	public void PopScope()
	{
		if (_scopeStack.Count > 0)
		{
			_scopeStack.Pop();
		}
	}
}
