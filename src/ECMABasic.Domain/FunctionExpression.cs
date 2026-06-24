using ECMABasic.Domain.Expressions;

namespace ECMABasic.Domain;

public class FunctionExpression : IExpression
{
	public FunctionExpression(string name, Func<IEnvironment, List<object>, object> fn, IEnumerable<IExpression> args)
	{
		Name = name;

		if (Name.EndsWith("$"))
		{
			Type = ExpressionType.String;
		}
		else
		{
			Type = ExpressionType.Number;
		}

		Function = fn;

		Arguments = new List<IExpression>(args);
	}

	public string Name { get; }

	public Func<IEnvironment, List<object>, object> Function { get; }

	public ExpressionType Type { get; }

	public List<IExpression> Arguments { get; }

	public bool IsReducible => Arguments.All(x => x.IsReducible);

	public object Evaluate(IEnvironment env)
	{
		// Pass environment to function delegate
		return Function(env, Arguments.Select(x => x.Evaluate(env)).ToList());
	}

	public string ToListing()
	{
		return string.Concat(Name, "(", string.Join(",", Arguments.Select(x => x.ToListing())), ")");
	}
}
