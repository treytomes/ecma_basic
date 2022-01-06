using ECMABasic.Core.Exceptions;
using ECMABasic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core.Statements
{
	public class InputStatement : IStatement
	{
		public InputStatement(IEnumerable<VariableExpression> variables)
		{
			Variables = new List<VariableExpression>(variables);
		}

		public List<VariableExpression> Variables { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			env.Print("? ");
			var reply = env.ReadLine();

			var reader = ComplexTokenReader.FromText(reply);

			var isFirst = true;
			foreach (var v in Variables)
			{
				reader.Next(TokenType.Space, false);

				if (!isFirst)
				{
					reader.Next(TokenType.Comma);
					reader.Next(TokenType.Space, false);
				}
				isFirst = false;

				var number = reader.NextNumber(false);
				if (number == null)
				{
					var token = reader.Next();
					if (token == null)
					{
						throw new RuntimeException("OUT OF INPUT", env.CurrentLineNumber);
					}

					var text = token.Text.Trim();
					if (token.Type == TokenType.String)
					{
						text = text.Substring(1, text.Length - 2);
					}

					// Assign a string.
					env.SetStringVariableValue(v.Name, text);
				}
				else
				{
					// Assign a number.
					env.SetNumericVariableValue(v.Name, number.Value);
				}

				reader.Next(TokenType.Space, false);
			}

			if (reader.Next() != null)
			{
				throw new RuntimeException("TOO MUCH INPUT", env.CurrentLineNumber);
			}
		}

		public string ToListing()
		{
			return string.Concat("INPUT ", string.Join(",", Variables.Select(x => x.ToListing())));
		}
	}
}
