using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : Statement
	{
		public PrintStatement(IEnumerable<IExpression> expr = null)
			: base(StatementType.PRINT)
		{
			Expressions = new List<IExpression>(expr ?? Enumerable.Empty<IExpression>());
		}

		/// <summary>
		/// The expressions to print.
		/// </summary>
		public List<IExpression> Expressions { get; }

		public override void Execute(IEnvironment env)
		{
			if (Expressions.Count == 0)
			{
				env.PrintLine();
				return;
			}

			foreach (var expr in Expressions)
			{
				var text = expr.Evaluate(env);
				env.Print(text);
			}

			if (!(Expressions.Last() is SemicolonExpression))
			{
				env.PrintLine();
			}
		}
	}
}
