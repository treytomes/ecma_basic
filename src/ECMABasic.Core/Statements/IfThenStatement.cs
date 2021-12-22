using System;

namespace ECMABasic.Core.Statements
{
	public class IfThenStatement : GotoStatement
	{
		public IfThenStatement(IExpression condition, IExpression lineNumber)
			: base(lineNumber)
		{
			Condition = condition;
		}

		public IExpression Condition { get; }

		public override void Execute(IEnvironment env)
		{
			var condition = Convert.ToBoolean(Condition.Evaluate(env));
			if (condition)
			{
				base.Execute(env);
			}
		}

		public override string ToListing()
		{
			return string.Concat("IF ", Condition.ToListing(), " THEN ", LineNumber.ToListing());
		}
	}
}
