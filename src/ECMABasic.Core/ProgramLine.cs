using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
    /// <summary>
    /// Represents a single line of a program.
    /// </summary>
    public class ProgramLine
    {
        /// <summary>
        /// Construct a program line instance.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="statement">The statement to execute on this line.</param>
        public ProgramLine(int lineNumber, Statement statement)
        {
            LineNumber = lineNumber;
            Statement = statement;
        }

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The statement to execute on this line.
        /// </summary>
        public Statement Statement { get; }
    }
}
