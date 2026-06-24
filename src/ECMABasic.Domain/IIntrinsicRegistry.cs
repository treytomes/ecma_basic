namespace ECMABasic.Domain;

/// <summary>
/// Registry for language intrinsic functions (built-in functions defined by ECMA-55).
/// Distinguishes intrinsic functions from user-defined functions (DEF FN).
/// </summary>
public interface IIntrinsicRegistry
{
	/// <summary>
	/// Register an intrinsic function with the given name, argument types, and implementation.
	/// </summary>
	/// <param name="name">The function name (e.g., "ABS", "SIN", "RND").</param>
	/// <param name="args">The expected argument types for this function.</param>
	/// <param name="fn">The implementation function that executes when called. Receives environment and evaluated arguments.</param>
	public void Register(string name, IEnumerable<ExpressionType> args, Func<IEnvironment, List<object>, object> fn);

	/// <summary>
	/// Get all function definitions with the given name.
	/// Multiple definitions may exist if overloaded by argument types.
	/// </summary>
	/// <param name="name">The function name to look for.</param>
	/// <returns>All matching function definitions.</returns>
	public IEnumerable<FunctionDefinition> Get(string name);
}
