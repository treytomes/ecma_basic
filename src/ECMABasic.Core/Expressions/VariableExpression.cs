using System;

namespace ECMABasic.Core.Expressions
{
	public class VariableExpression : IExpression
	{
		public VariableExpression(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public bool IsString
		{
			get
			{
				return Name.EndsWith('$');
			}
		}

		public bool IsNumeric
		{
			get
			{
				return !IsString;
			}
		}

		public ExpressionType Type
		{
			get
			{
				if (IsNumeric)
				{
					return ExpressionType.Number;
				}
				else
				{
					return ExpressionType.String;
				}
			}
		}

		public object Evaluate(IEnvironment env)
		{
			if (IsString)
			{
				return env.GetStringVariableValue(Name);
			}
			else if (IsNumeric)
			{
				return env.GetNumericVariableValue(Name);
			}
			throw new InvalidOperationException("This shouldn't have happened.");
		}

		public string ToListing()
		{
			return Name;
		}
	}
}
