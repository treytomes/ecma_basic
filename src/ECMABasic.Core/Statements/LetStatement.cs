using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Statements
{
	public class LetStatement : Statement
	{
		public LetStatement(VariableExpression targetExpr, IExpression valueExpr)
			: base(StatementType.LET)
		{
			Target = targetExpr;
			Value = valueExpr;
		}

		public VariableExpression Target { get; }
		public IExpression Value { get; }

		public override void Execute(IEnvironment env)
		{
			var value = Value.Evaluate(env);
			env.SetStringVariableValue(Target.Name, value);
		}
	}
}
