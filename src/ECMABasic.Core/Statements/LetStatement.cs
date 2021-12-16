using ECMABasic.Core.Expressions;
using System;

namespace ECMABasic.Core.Statements
{
	public class LetStatement : IStatement
	{
		public LetStatement(VariableExpression targetExpr, IExpression valueExpr)
		{
			Target = targetExpr;
			Value = valueExpr;
		}

		public VariableExpression Target { get; }
		public IExpression Value { get; }

		public void Execute(IEnvironment env)
		{
			var value = Value.Evaluate(env);
			if (Target.IsString)
			{
				env.SetStringVariableValue(Target.Name, Convert.ToString(value));
			}
			else
			{
				env.SetNumericVariableValue(Target.Name, Convert.ToDouble(value));
			}
		}

		public string ToListing()
		{
			return string.Concat("LET ", Target.ToListing(), "=", Value.ToListing());
		}
	}
}
