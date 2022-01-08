using System;

namespace ECMABasic.Core
{
    /// <summary>
    /// Represents a single line of a program.
    /// </summary>
    public class ProgramLine : IListable
    {
        /// <summary>
        /// Construct a program line instance.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="statement">The statement to execute on this line.</param>
        public ProgramLine(int lineNumber, IStatement statement, ProgramLine parent)
        {
            LineNumber = lineNumber;
            Statement = statement;
            Parent = parent;
        }

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The statement to execute on this line.
        /// </summary>
        public IStatement Statement { get; }

        /// <summary>
        /// Is this line contained in a block?
        /// </summary>
        public ProgramLine Parent { get; }

        public string ToListing()
		{
			return string.Concat(LineNumber.ToString(), " ", Statement.ToListing(), Environment.NewLine);
		}
	}
}
