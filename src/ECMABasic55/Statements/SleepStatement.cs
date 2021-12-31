using ECMABasic.Core;
using System;
using System.Threading;

namespace ECMABasic55.Statements
{
	public class SleepStatement : IStatement
	{
		public SleepStatement(IExpression milliseconds)
		{
			Milliseconds = milliseconds;
		}

		public IExpression Milliseconds { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			Thread.Sleep(Convert.ToInt32(Milliseconds.Evaluate(env)));
		}

		public string ToListing()
		{
			return string.Concat("SLEEP ", Milliseconds.ToListing());
		}
	}
}
