using System;
using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
using ECMABasic.Domain.Exceptions;

namespace ECMABasic.Application;

/// <summary>
/// Expression for calling a user-defined function (DEF FN).
/// ECMA-55 Section 10: User-defined numeric functions.
/// </summary>
public class UserFunctionCallExpression : IExpression
{
	private readonly string _functionName;
	private readonly IExpression? _argument;
	private readonly int? _lineNumber;

	public UserFunctionCallExpression(string functionName, IExpression? argument, int? lineNumber)
	{
		_functionName = functionName;
		_argument = argument;
		_lineNumber = lineNumber;
	}

	public ExpressionType Type => ExpressionType.Number;

	public bool IsReducible => false;

	public object Evaluate(IEnvironment env)
	{
		// ECMA55-DEF-005: Definition must appear before first reference
		var function = env.Functions.Get(_functionName);
		if (function == null)
		{
			throw new RuntimeException($"Undefined function {_functionName}", env.CurrentLineNumber);
		}

		// Evaluate argument if provided
		object? argumentValue = null;
		if (_argument != null)
		{
			argumentValue = _argument.Evaluate(env);
		}

		// For zero-parameter functions, just evaluate the body
		if (function.Parameter == null)
		{
			if (argumentValue != null)
			{
				throw new RuntimeException($"Function {_functionName} takes no parameters", env.CurrentLineNumber);
			}

			// ECMA55-DEF-004: Non-parameter variables refer to global scope
			return function.Body.Evaluate(env);
		}

		// For one-parameter functions, use scope stack
		// ECMA55-DEF-002: Function parameter shadows global variables
		if (argumentValue == null)
		{
			throw new RuntimeException($"Function {_functionName} requires one parameter", env.CurrentLineNumber);
		}

		var paramValue = Convert.ToDouble(argumentValue);
		var envBase = (EnvironmentBase)env;

		envBase.PushScope(function.Parameter, paramValue);
		try
		{
			// ECMA55-DEF-004: Non-parameter variables still refer to global scope
			return function.Body.Evaluate(env);
		}
		finally
		{
			envBase.PopScope();
		}
	}

	public string ToListing()
	{
		if (_argument != null)
		{
			return $"{_functionName}({_argument.ToListing()})";
		}
		return _functionName;
	}
}
