using ECMABasic.Infrastructure;
using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Exceptions;

namespace ECMABasic55.Statements;

public class NewStatement : IStatement
{
	public void Execute(IEnvironment env, bool isImmediate)
	{
		if (!isImmediate)
		{
			throw ECMABasic.Application.ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
		}

		env.Clear();
	}

	public string ToListing()
	{
		return "NEW";
	}
}
