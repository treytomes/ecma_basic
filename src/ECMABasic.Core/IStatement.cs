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
    public interface IStatement
    {
        public abstract void Execute(IEnvironment env);
    }
}
