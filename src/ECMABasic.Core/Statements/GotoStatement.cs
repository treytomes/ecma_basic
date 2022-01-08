using System;

namespace ECMABasic.Core.Statements
{
	// TODO: # GOTO # needs to start an infinite loop.

	public class GotoStatement : IStatement
	{
		public GotoStatement(IExpression lineNumber)
		{
			if (lineNumber.Type != ExpressionType.Number)
			{
				throw ExceptionFactory.ExpectedNumericExpression();
			}
			LineNumber = lineNumber;
		}

		public IExpression LineNumber { get; }

		/// <summary>
		/// The <paramref name="src"/> parent tree needs to contain the parent of <paramref name="dst"/>.
		/// </summary>
		/// <param name="src">The line jumping from.</param>
		/// <param name="dst">The line jumping to.</param>
		private void ValidateSharedAncestry(IEnvironment env, ProgramLine src, ProgramLine dst)
		{
			if (dst.Parent == null)
			{
				return;
			}

			if (src == null)
			{
				// We travelled all the way up without finding dst.Parent.
				throw ExceptionFactory.ControlTransferIntoForBlock(env.CurrentLineNumber);
			}

			if (src.Parent == dst.Parent)
			{
				return;
			}

			ValidateSharedAncestry(env, src.Parent, dst);
		}

		public virtual void Execute(IEnvironment env, bool isImmediate)
		{
			if (isImmediate)
			{
				throw ExceptionFactory.OnlyAllowedInProgram();
			}

			var thisLine = env.Program[env.CurrentLineNumber];

			var lineNumber = Convert.ToInt32(LineNumber.Evaluate(env));
			if (!env.ValidateLineNumber(lineNumber, false))
			{
				throw ExceptionFactory.UndefinedLineNumber(lineNumber, env.CurrentLineNumber);
			}

			var newLine = env.Program[lineNumber];
			ValidateSharedAncestry(env, thisLine, newLine);
			env.CurrentLineNumber = lineNumber;

			var context = env.PopCallStack();
			if (context is ForStackContext)
			{
				var forContext = context as ForStackContext;
				if ((env.CurrentLineNumber < forContext.BlockStartLineNumber) || (env.CurrentLineNumber >= forContext.BlockEndLineNumber))
				{
					// We just jumped out of the FOR-block, so we can dispose of the context.
				}
				else
				{
					env.PushCallStack(context);
				}
			}
			else
			{
				if (context != null)
				{
					env.PushCallStack(context);
				}
			}
		}

		public virtual string ToListing()
		{
			return string.Concat("GOTO ", LineNumber.ToListing());
		}
	}
}
