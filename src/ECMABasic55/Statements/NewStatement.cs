using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Core;
using ECMABasic.Core.Exceptions;

namespace ECMABasic55.Statements;

public class NewStatement : IStatement
{
	public void Execute(IEnvironment env, bool isImmediate)
	{
		if (!isImmediate)
		{
			throw ECMABasic.Core.ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
		}

		env.Clear();
	}

	public string ToListing()
	{
		return "NEW";
	}
}
