using ECMABasic.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class DataStatement : IStatement
	{
		public DataStatement(IEnumerable<IExpression> datums)
		{
			if (!datums.All(x => x.IsReducible))
			{
				throw new SyntaxException("DATUMS MUST BE REDUCIBLE TO STRING OR NUMERIC CONSTANTS");
			}

			Datums = new List<IExpression>(datums);
		}

		public List<IExpression> Datums { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}
		}

		public string ToListing()
		{
			return string.Concat("DATA ", string.Join(",", Datums.Select(x => x.ToListing())));
		}
	}
}
