using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		private const int NUM_SIGNIFICANT_DIGITS = 6;
		private readonly static string _doubleFormat = string.Format(" .{0} ;-.{0} ; 0 ", new string('#', NUM_SIGNIFICANT_DIGITS));

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

			//if (env.CurrentLineNumber == 200)
			//{
			//	var a = 0;
			//}

			foreach (var expr in PrintItems)
			{
				var value = expr.Evaluate(env);
				string text;

				if (value is int)
				{
					text = PrintInteger((int)value);
				}
				else if (value is double)
				{
					text = PrintDouble((double)value);
				}
				else
				{
					text = Convert.ToString(value);
				}

				env.Print(text);
			}

			if (!(PrintItems.Last() is SemicolonExpression))
			{
				env.PrintLine();
			}
		}

		private string PrintInteger(int value)
		{
			var sign = (value > 0) ? " " : "";
			return sign + value.ToString() + " ";
		}

		private string PrintDouble(double value)
		{
			return value.ToString(_doubleFormat);
		}
	}
}
