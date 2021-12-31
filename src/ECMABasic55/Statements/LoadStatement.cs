using ECMABasic.Core;
using ECMABasic.Core.Exceptions;
using System;
using System.IO;

namespace ECMABasic55.Statements
{
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
				throw new SyntaxException("NOT ALLOWED IN PROGRAM");
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
}
