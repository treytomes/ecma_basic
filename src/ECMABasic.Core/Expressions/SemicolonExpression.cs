using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Expressions
{
	/// <summary>
	/// Used when a semi-colon occurs in a print list.
	/// </summary>
	public class SemicolonExpression : IExpression
	{
		public string Evaluate(IEnvironment env)
		{
			// Nothing really to do here.  Printing will naturally move to the next column on it's own.
			return string.Empty;
		}
	}
}
