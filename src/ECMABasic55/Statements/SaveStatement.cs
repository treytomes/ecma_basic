using ECMABasic.Core;
using ECMABasic.Core.Exceptions;
using System;
using System.IO;

namespace ECMABasic55.Statements
{
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
				throw ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
			}

			var path = Convert.ToString(Path.Evaluate(env));

			var contents = env.Program.ToListing();
			File.WriteAllText(path, contents);
		}

		public string ToListing()
		{
			return string.Concat("SAVE ", Path.ToListing());
		}
	}
}
