using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Expressions
{
	class StringExpression : IExpression
	{
		public StringExpression(string text)
			: base()
		{
			Text = text;
		}

		public string Text { get; }

		public object Evaluate(IEnvironment env)
		{
			return Text;
		}
	}
}
