using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	public interface IExpression
	{
		/// <summary>
		/// Convert the expression into text.
		/// </summary>
		/// <param name="env">The environment to evaluate against.</param>
		/// <returns>The text representation of this expression.</returns>
		string Evaluate(IEnvironment env);
	}
}
