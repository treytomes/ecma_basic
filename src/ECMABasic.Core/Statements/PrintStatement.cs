using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		private const int NUM_SIGNIFICANT_DIGITS = 6;
		private readonly static string _largeNumberFormat = string.Format(" .{0}E+# ;-.{0}E+# ", new string('#', NUM_SIGNIFICANT_DIGITS));
		private readonly static string _numberFormat = string.Format(" .{0} ;-.{0} ; 0 ", new string('#', NUM_SIGNIFICANT_DIGITS));

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

			//if (env.CurrentLineNumber == 1400)
			//{
			//	var a = 0;
			//}

			foreach (var expr in PrintItems)
			{
				var value = expr.Evaluate(env);
				string text = value switch
				{
					int => PrintInteger((int)value),
					double => PrintDouble((double)value),
					_ => Convert.ToString(value),
				};
				env.Print(text);
			}

			if (!(PrintItems.Last() is SemicolonExpression))
			{
				env.PrintLine();
			}
		}

		private static string PrintInteger(int value)
		{
			var sign = (value > 0) ? " " : "";
			return sign + value.ToString() + " ";
		}

		private static string PrintDouble(double value)
		{
			if (value == -0.0)
			{
				value = 0.0;
			}

			// TODO: G6 replacement? 0.000000E0##
			///var expFormat = "G6";
			var expFormat = "#.######E+0##";

			string text;
			if ((value > 0) && ((value > 999999) || (value < 0.000001)))
			{
				text = " " + value.ToString(expFormat) + " ";
			}
			else if ((value < 0) && ((value < -999999) || (value > -0.000001)))
			{
				text = value.ToString(expFormat) + " ";
			}
			else
			{
				text = value.ToString(_numberFormat);
			}

			var dotIndex = text.IndexOf('.');
			var eIndex = text.IndexOf('E');
			if (dotIndex >= 0)
			{
				if (eIndex >= 0)
				{
					if ((eIndex - dotIndex + 1 == 1) && (text[dotIndex + 1] == '0'))
					{
						// There's a single character, and it's a 0.
						text = string.Concat(text.Substring(0, dotIndex), text[eIndex..]);
					}
				}
				else
				{
					eIndex = text.Length;
					if ((eIndex - dotIndex + 1 == 1) && (text[dotIndex + 1] == '0'))
					{
						// There's a single character, and it's a 0.
						text = string.Concat(text.Substring(0, dotIndex), text[eIndex..]);
					}
				}
			}
			else if (eIndex >= 0)
			{
				text = string.Concat(text.Substring(0, eIndex), ".", text[eIndex..]);
			}

			return text;
		}
	}
}
