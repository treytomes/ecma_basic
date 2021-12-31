using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	public abstract class FunctionExpression : IExpression
	{
		public FunctionExpression(IEnumerable<IExpression> args)
		{
			Arguments = new List<IExpression>(args);
		}

		public abstract string Name { get; }

		public abstract ExpressionType Type { get; }

		public List<IExpression> Arguments { get; }

		public abstract object Evaluate(IEnvironment env);

		public string ToListing()
		{
			return string.Concat(Name, "(", string.Join(",", Arguments.Select(x => x.ToListing())), ")");
		}
	}
}
