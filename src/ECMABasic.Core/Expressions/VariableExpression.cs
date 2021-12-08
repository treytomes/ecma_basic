using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Expressions
{
	public class VariableExpression : IExpression
	{
		public VariableExpression(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public string Evaluate(IEnvironment env)
		{
			return env.GetStringVariableValue(Name);
		}
	}
}
