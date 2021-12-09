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
		/// Convert the expression a base type.
		/// </summary>
		/// <param name="env">The environment to evaluate against.</param>
		/// <returns>The text representation of this expression.</returns>
		object Evaluate(IEnvironment env);
	}
}
