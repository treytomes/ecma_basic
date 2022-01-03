using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class ReadStatement : IStatement
	{
		public ReadStatement(IEnumerable<VariableExpression> variables)
		{
			Variables = new List<VariableExpression>(variables);
		}

		public List<VariableExpression> Variables { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			foreach (var v in Variables)
			{
				var datum = env.ReadData();

				if (datum.Type != v.Type)
				{
					throw new SyntaxException("MIXED STRINGS AND NUMBERS");
				}

				var datumValue = datum.Evaluate(env);

				if (v.IsString)
				{
					env.SetStringVariableValue(v.Name, Convert.ToString(datumValue));
				}
				else
				{
					env.SetNumericVariableValue(v.Name, Convert.ToDouble(datumValue));
				}
			}
		}

		public string ToListing()
		{
			return string.Concat("READ ", string.Join(",", Variables.Select(x => x.ToListing())));
		}
	}
}
