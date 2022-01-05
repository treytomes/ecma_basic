using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core.Statements
{
	public class ReturnStatement : IStatement
	{
		public ReturnStatement()
		{
		}

		// TODO: Need a "% RETURN WITHOUT GOSUB".  Without the line # if running as an immediate statement.

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}

			try
			{
				while (true)
				{
					var context = env.PopCallStack();
					if (context is GosubStackContext)
					{
						env.CurrentLineNumber = (context as GosubStackContext).LineNumber;
						break;
					}
				}
			}
			catch (InvalidOperationException)
			{
				throw ExceptionFactory.ReturnWithoutGosub(env.CurrentLineNumber);
			}
		}

		public string ToListing()
		{
			return "RETURN";
		}
	}
}
