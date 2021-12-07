using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
    /// <summary>
    /// Represents a single executable statement.
    /// </summary>
    public abstract class Statement
    {
        /// <summary>
        /// Construct a statement instance.
        /// </summary>
        /// <param name="type">The type of this statement.</param>
        public Statement(StatementType type)
        {
            Type = type;
        }

        /// <summary>
        /// The type of this statement.
        /// </summary>
        public StatementType Type { get; }

        public abstract void Execute(IEnvironment env);
    }
}
