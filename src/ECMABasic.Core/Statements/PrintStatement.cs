using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		public PrintStatement(IEnumerable<IPrintItem> expr = null)
		{
			PrintItems = new List<IPrintItem>(expr ?? Enumerable.Empty<IPrintItem>());
		}

		/// <summary>
		/// The expressions to print.
		/// </summary>
		public List<IPrintItem> PrintItems { get; }

		public void Execute(IEnvironment env)
		{
			if (PrintItems.Count == 0)
			{
				env.PrintLine();
				return;
			}

			foreach (var expr in PrintItems)
			{
				var text = Convert.ToString(expr.Evaluate(env));
				env.Print(text);
			}

			if (!(PrintItems.Last() is SemicolonExpression))
			{
				env.PrintLine();
			}
		}
	}
}
