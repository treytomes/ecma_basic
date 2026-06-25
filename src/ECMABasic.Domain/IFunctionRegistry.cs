namespace ECMABasic.Domain;

/// <summary>
/// Registry for user-defined functions (DEF FN).
/// ECMA-55 Section 10: User-defined numeric functions.
/// </summary>
public interface IFunctionRegistry
{
	/// <summary>
	/// Define a user function.
	/// ECMA55-DEF-001: DEF FNx = expression or DEF FNx(parameter) = expression
	/// </summary>
	/// <param name="name">Function name (single letter A-Z)</param>
	/// <param name="parameter">Optional parameter name (simple numeric variable)</param>
	/// <param name="body">Expression to evaluate when function is called</param>
	public void Define(string name, string? parameter, IExpression body);

	/// <summary>
	/// Check if a function is defined.
	/// Used for ECMA55-DEF-005 validation (use-before-def).
	/// </summary>
	/// <param name="name">Function name to check</param>
	/// <returns>True if function is defined</returns>
	public bool IsDefined(string name);

	/// <summary>
	/// Get a user function definition.
	/// </summary>
	/// <param name="name">Function name</param>
	/// <returns>Function definition, or null if not defined</returns>
	public UserFunction? Get(string name);
}

/// <summary>
/// User-defined function definition.
/// </summary>
public record UserFunction(string Name, string? Parameter, IExpression Body);
