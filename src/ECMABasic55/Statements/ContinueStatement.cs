using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Exceptions;
using ECMABasic.Application.Statements;

namespace ECMABasic55.Statements;

/// <summary>
/// Continue after a program has been stopped.
/// </summary>
public class ContinueStatement : IStatement
{
	// TODO: ?CN ERROR if there if the program wasn't STOPped.

	public void Execute(IEnvironment env, bool isImmediate)
	{
		if (!isImmediate)
		{
			throw ECMABasic.Application.ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
		}

		var currentLine = ((EnvironmentBase)env).Program[env.CurrentLineNumber];
		if (currentLine != null && currentLine.Statement is StopStatement)
		{
			((EnvironmentBase)env).Program.MoveToNextLine(env);
		}
		((EnvironmentBase)env).Program.Execute(env);
	}

	public string ToListing()
	{
		return "CONT";
	}
}
