using ECMABasic.Infrastructure;
using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Exceptions;
using System;
using System.IO;

namespace ECMABasic55.Statements;

public class LoadStatement : IStatement
{
	public LoadStatement(IExpression path)
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
		if (!File.Exists(path))
		{
			env.ReportError("% FILE NOT FOUND");
			return;
		}

		env.LoadFile(path);
	}

	public string ToListing()
	{
		return string.Concat("LOAD ", Path.ToListing());
	}
}
