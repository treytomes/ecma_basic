using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		private const int SIGNIFICANCE_WIDTH = 6;
		private const int EXRAD_WIDTH = 2;
		private readonly static string _scientificFormat = string.Format(" #.#####E+0 ;-#.#####E+0 ", new string('#', SIGNIFICANCE_WIDTH), new string('0', EXRAD_WIDTH));
		private readonly static string _numberFormat = string.Format(" .{0} ;-.{0} ; 0 ", new string('#', SIGNIFICANCE_WIDTH));

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

			//if (env.CurrentLineNumber == 240)
			//{
			//	var a = 0;
			//}

			foreach (var expr in PrintItems)
			{
				var value = expr.Evaluate(env);
				string text = value switch
				{
					int => PrintNumber((double)value),
					double => PrintNumber((double)value),
					_ => Convert.ToString(value),
				};
				env.Print(text);
			}

			if (!(PrintItems.Last() is IPrintItemSeparator))
			{
				env.PrintLine();
			}
		}

		private static string PrintNumber(double value)
		{
			if ((value == 0) || (value == -0.0))
			{
				return " 0 ";
			}

			var numDigits = Math.Floor(Math.Log10(Math.Abs(value)));
			var scale = (double)Math.Pow(10, numDigits + 1);  // Calculate the scale of the number.
			var newValue = (double)(scale * Math.Round((double)value / scale, SIGNIFICANCE_WIDTH));  // Reduce the significant digit width.
			if (numDigits == -SIGNIFICANCE_WIDTH)
			{
				if (Math.Round(value, SIGNIFICANCE_WIDTH) == value)
				{
					return newValue.ToString(_numberFormat);
				}
				else
				{
					return newValue.ToString(_scientificFormat);
				}
			}
			else if ((numDigits > -SIGNIFICANCE_WIDTH) && (numDigits <= SIGNIFICANCE_WIDTH))
			{
				return newValue.ToString(_numberFormat);
			}
			else
			{
				var text = newValue.ToString(_scientificFormat);
				var eIndex = text.IndexOf("E");
				if (eIndex >= 0)
				{
					var dotIndex = text.IndexOf(".");
					if (dotIndex < 0)
					{
						// There should always be a full-stop between the integer and the E.
						text = text.Insert(eIndex, ".");
					}
				}
				return text;
			}
		}
	}
}
