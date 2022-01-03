using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class DataStatement : IStatement
	{
		public DataStatement(IEnumerable<IExpression> datums)
		{
			if (!datums.All(x => (x is NumberExpression) || (x is StringExpression)))
			{
				throw new SyntaxException("DATUMS MUST BE STRING OR NUMERIC CONSTANTS");
			}

			Datums = new List<IExpression>(datums);
		}

		public List<IExpression> Datums { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw new SyntaxException("MUST BE IN A PROGRAM");
			}
		}

		public string ToListing()
		{
			return string.Concat("DATA ", string.Join(",", Datums.Select(x => x.ToListing())));
		}
	}
}
