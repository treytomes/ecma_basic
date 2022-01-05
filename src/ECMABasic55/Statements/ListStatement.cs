using ECMABasic.Core;
using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using System;
using System.Linq;
using System.Text;

namespace ECMABasic55.Statements
{
	public class ListStatement : IStatement
	{
		private readonly IBasicConfiguration _config;

		public ListStatement(IExpression from, IExpression to, IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
			From = from;
			To = to;
		}

		public IExpression From { get; }
		public IExpression To { get; }

		public void Execute(IEnvironment env, bool isImmediate)
		{
			if (!isImmediate)
			{
				throw ExceptionFactory.NotAllowedInProgram(env.CurrentLineNumber);
			}

			if (env.Program.Length == 0)
			{
				return;
			}

			var fromLineNumber = (int)((From == null) ? env.Program.First().LineNumber : Convert.ToInt32(From.Evaluate(env)));
			if (fromLineNumber < 0)
			{
				throw ExceptionFactory.LineNumberOutOfRange(fromLineNumber, env.CurrentLineNumber);
			}

			if ((From != null) && (To == null))
			{
				env.ValidateLineNumber(fromLineNumber, true);
				env.Print(env.Program[fromLineNumber].ToListing());
				return;
			}

			var toLineNumber = (To == null) ? env.Program.Last().LineNumber : Convert.ToInt32(To.Evaluate(env));
			if ((toLineNumber < fromLineNumber) || (toLineNumber > _config.MaxLineNumber))
			{
				throw ExceptionFactory.LineNumberOutOfRange(fromLineNumber, env.CurrentLineNumber);
			}

			foreach (var line in env.Program)
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
}
