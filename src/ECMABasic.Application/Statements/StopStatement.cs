using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Domain.Exceptions;

namespace ECMABasic.Application.Statements;

public class StopStatement : IStatement
{
	public void Execute(IEnvironment env, bool isImmediate)
	{
		throw ExceptionFactory.ProgramStop(env.CurrentLineNumber);
	}

	// TODO: Centralize keyword strings to make them easier to change?

	public string ToListing()
	{
		return "STOP";
	}
}
