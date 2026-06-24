using ECMABasic.Infrastructure;
using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Exceptions;
using System;
using System.IO;

namespace ECMABasic55.Statements;

public class SaveStatement : IStatement
{
	public SaveStatement(IExpression path)
	{
		Path = path;
	}

	public IExpression Path { get; }

	public void Execute(IEnvironment env, bool isImmediate)
	{
		if (!isImmediate)
		{
			throw ECMABasic.Application.ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
		}

		var path = Convert.ToString(Path.Evaluate(env));
		if (string.IsNullOrEmpty(path))
		{
			throw ECMABasic.Application.ExceptionFactory.Syntax();
		}

		var contents = ((EnvironmentBase)env).Program.ToListing();
		File.WriteAllText(path, contents);
	}

	public string ToListing()
	{
		return string.Concat("SAVE ", Path.ToListing());
	}
}
