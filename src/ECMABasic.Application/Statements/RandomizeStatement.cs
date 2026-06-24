using ECMABasic.Domain;

namespace ECMABasic.Application.Statements;

/// <summary>
/// RANDOMIZE statement implementation per ECMA-55 specification.
/// ECMA55-RND-001: Generates unpredictable starting point for RND sequence.
/// </summary>
public class RandomizeStatement : IStatement
{
	public void Execute(IEnvironment env, bool isRunMode)
	{
		env.Random.Randomize();
	}

	public string ToListing()
	{
		return "RANDOMIZE";
	}
}
