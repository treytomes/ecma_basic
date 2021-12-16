using System;
using System.IO;

namespace ECMABasic.Core.Statements
{
	public class LoadStatement : IStatement
	{
		public LoadStatement(IExpression path)
		{
			Path = path;
		}

		public IExpression Path { get; }

		public void Execute(IEnvironment env)
		{
			var path = Convert.ToString(Path.Evaluate(env));
			if (!File.Exists(path))
			{
				env.ReportError("% FILE NOT FOUND");
				return;
			}

			env.Program.Clear();
			Interpreter.FromFile(path, env);
		}
	}
}
