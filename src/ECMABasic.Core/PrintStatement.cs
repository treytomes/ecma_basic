using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	public class PrintStatement : Statement
	{
		public PrintStatement(IExpression expr)
			: base(StatementType.PRINT)
		{
			Expression = expr;
		}

		/// <summary>
		/// The expression to print.
		/// </summary>
		public IExpression Expression { get; }

		public override void Execute(IEnvironment env)
		{
			env.PrintLine(Expression.ToString());
		}
	}
}
