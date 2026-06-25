using System.Collections.Generic;
using ECMABasic.Domain;
using ECMABasic.Domain.Exceptions;

namespace ECMABasic.Application;

/// <summary>
/// Registry for user-defined functions (DEF FN).
/// ECMA-55 Section 10: User-defined numeric functions.
/// </summary>
public class FunctionRegistry : IFunctionRegistry
{
	private readonly Dictionary<string, UserFunction> _functions = new();

	public void Define(string name, string? parameter, IExpression body)
	{
		// ECMA55-DEF-008: Function defined at most once per program
		if (_functions.ContainsKey(name))
		{
			throw new RuntimeException($"Function {name} is already defined");
		}

		_functions[name] = new UserFunction(name, parameter, body);
	}

	public bool IsDefined(string name)
	{
		return _functions.ContainsKey(name);
	}

	public UserFunction? Get(string name)
	{
		return _functions.TryGetValue(name, out var function) ? function : null;
	}
}
