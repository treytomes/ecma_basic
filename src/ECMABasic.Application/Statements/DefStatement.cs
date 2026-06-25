using ECMABasic.Domain;

namespace ECMABasic.Application.Statements;

/// <summary>
/// DEF FN statement for user-defined functions.
/// ECMA-55 Section 10: User-defined numeric functions.
/// </summary>
public class DefStatement : IStatement
{
	public DefStatement(string name, string? parameter, IExpression body)
	{
		Name = name;
		Parameter = parameter;
		Body = body;
	}

	public string Name { get; }
	public string? Parameter { get; }
	public IExpression Body { get; }

	public void Execute(IEnvironment env, bool isRunMode)
	{
		// ECMA55-DEF-006: Executing DEF line has no effect (just register and continue)
		env.Functions.Define(Name, Parameter, Body);
	}

	public string ToListing()
	{
		var param = Parameter != null ? $"({Parameter})" : "";
		return $"DEF {Name}{param} = {Body.ToListing()}";
	}
}
