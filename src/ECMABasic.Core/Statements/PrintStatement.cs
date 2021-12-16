using ECMABasic.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class PrintStatement : IStatement
	{
		private readonly string _scientificFormat;
		private readonly string _numberFormat;

		private readonly IBasicConfiguration _config;

		public PrintStatement(IEnumerable<IPrintItem> expr = null, IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
			if (_scientificFormat == null)
			{
				_scientificFormat = string.Format(" #.{0}E+{1} ;-#.{0}E+{1} ", new string('#', _config.SignificanceWidth - 1), new string('0', _config.ExradWidth - 1));
			}
			if (_numberFormat == null)
			{
				_numberFormat = string.Format(" .{0} ;-.{0} ; 0 ", new string('#', _config.SignificanceWidth));
			}

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

		public string ToListing()
		{
			return string.Concat("PRINT ", string.Join(string.Empty, PrintItems.Select(x => x.ToListing())));
		}

		private string PrintNumber(double value)
		{
			if ((value == 0) || (value == -0.0))
			{
				return " 0 ";
			}

			var numDigits = Math.Floor(Math.Log10(Math.Abs(value)));
			var scale = (double)Math.Pow(10, numDigits + 1);  // Calculate the scale of the number.
			var newValue = (double)(scale * Math.Round((double)value / scale, _config.SignificanceWidth));  // Reduce the significant digit width.
			if (numDigits == -_config.SignificanceWidth)
			{
				if (Math.Round(value, _config.SignificanceWidth) == value)
				{
					return newValue.ToString(_numberFormat);
				}
				else
				{
					return newValue.ToString(_scientificFormat);
				}
			}
			else if ((numDigits > -_config.SignificanceWidth) && (numDigits <= _config.SignificanceWidth))
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
