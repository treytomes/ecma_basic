using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		public PrintStatement(IEnumerable<IExpression> expr = null)
		{
			Expressions = new List<IExpression>(expr ?? Enumerable.Empty<IExpression>());
		}

		/// <summary>
		/// The expressions to print.
		/// </summary>
		public List<IExpression> Expressions { get; }

		public void Execute(IEnvironment env)
		{
			if (Expressions.Count == 0)
			{
				env.PrintLine();
				return;
			}

			foreach (var expr in Expressions)
			{
				var text = Convert.ToString(expr.Evaluate(env));
				env.Print(text);
			}

			if (!(Expressions.Last() is SemicolonExpression))
			{
				env.PrintLine();
			}
		}
	}
}
