using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Application;
using ECMABasic.Application.Configuration;
using ECMABasic.Application.Exceptions;
using System;
using System.Linq;
using System.Text;

namespace ECMABasic55.Statements;

public class ListStatement : IStatement
{
	private readonly IBasicConfiguration _config;

	public ListStatement(IExpression? from, IExpression? to, IBasicConfiguration? config = null)
	{
		_config = config ?? MinimalBasicConfiguration.Instance;
		From = from;
		To = to;
	}

	public IExpression? From { get; }
	public IExpression? To { get; }

	public void Execute(IEnvironment env, bool isImmediate)
	{
		if (!isImmediate)
		{
			throw ECMABasic.Application.ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
		}

		if (((EnvironmentBase)env).Program.Length == 0)
		{
			return;
		}

		var fromLineNumber = (int)((From == null) ? ((EnvironmentBase)env).Program.First().LineNumber : Convert.ToInt32(From.Evaluate(env)));
		if (fromLineNumber < 0)
		{
			throw ECMABasic.Application.ExceptionFactory.LineNumberOutOfRange(fromLineNumber, env.CurrentLineNumber);
		}

		if ((From != null) && (To == null))
		{
			env.ValidateLineNumber(fromLineNumber, true);
			var line = ((EnvironmentBase)env).Program[fromLineNumber];
			if (line != null)
			{
				env.Print(line.ToListing());
			}
			return;
		}

		var toLineNumber = (To == null) ? ((EnvironmentBase)env).Program.Last().LineNumber : Convert.ToInt32(To.Evaluate(env));
		if ((toLineNumber < fromLineNumber) || (toLineNumber > _config.MaxLineNumber))
		{
			throw ECMABasic.Application.ExceptionFactory.LineNumberOutOfRange(fromLineNumber, env.CurrentLineNumber);
		}

		foreach (var line in ((EnvironmentBase)env).Program)
		{
			if (line.LineNumber < fromLineNumber)
			{
				continue;
			}
			else if (line.LineNumber > toLineNumber)
			{
				break;
			}
			else
			{
				env.Print(line.ToListing());
			}
		}
	}

	public string ToListing()
	{
		var sb = new StringBuilder();
		sb.Append("LIST");
		if (From != null)
		{
			sb.AppendFormat(" {0}", From.ToListing());
		}
		if (To != null)
		{
			sb.AppendFormat("-{0}", To.ToListing());
		}
		return sb.ToString();
	}
}
