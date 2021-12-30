using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class OnGotoStatement : IStatement
	{
		public OnGotoStatement(IExpression value, IEnumerable<IExpression> branches)
		{
			if ((value.Type != ExpressionType.Number) || branches.Any(x => x.Type != ExpressionType.Number))
			{
				throw new SyntaxException("EXPECTED A NUMERIC EXPRESSION");
			}
			Value = value;
			Branches = new List<IExpression>(branches);
		}

		public IExpression Value { get; }
		public List<IExpression> Branches { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			// Subtract 1 from value because BASIC arrays start with 1.
			var value = Convert.ToInt32(Value.Evaluate(env)) - 1;
			if ((value < 0) || (value >= Branches.Count))
			{
				throw new RuntimeException("INDEX OUT OF RANGE", env.CurrentLineNumber);
			}

			var lineNumber = Convert.ToInt32(Branches[value].Evaluate(env));
			env.CurrentLineNumber = lineNumber;
		}

		public string ToListing()
		{
			return string.Concat("ON ", Value.ToListing(), " GOTO ", string.Join(",", Branches.Select(x => x.ToListing())));
		}
	}
}
