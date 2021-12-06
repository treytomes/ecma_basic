using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
    // TODO: The end-goal is to have a single interpreter that you can plug feature sets into, like adding BASIC-1 on top of Minimal BASIC.

    /// <summary>
    /// Attempt to interpret a token stream.
    /// </summary>
    public class Interpreter
    {
        private bool ProcessProgram()
        {
            while (ProcessBlock()) { }

            return ProcessEndLine();
        }

        private bool ProcessBlock()
        {
            if (ProcessLine())
            {
                return true;
            }
            if (ProcessForBlock())
            {
                return true;
            }
            return false;
        }

        private bool ProcessLine()
        {
            if (!ProcessLineNumber())
            {
                return false;
            }

            if (!ProcessStatement())
            {
                return false;
            }

            if (!ProcessEndOfLine())
            {
                return false;
            }

            return true;
        }

        private bool ProcessLineNumber()
        {
            throw new NotImplementedException();
        }

        private bool ProcessEndOfLine()
        {
            throw new NotImplementedException();
        }

        private bool ProcessEndLine()
        {
            if (!ProcessLineNumber())
            {
                return false;
            }

            if (!ProcessStatementType(StatementType.END))
            {
                return false;
            }

            if (!ProcessEndOfLine())
            {
                return false;
            }

            return true;
        }

        private bool ProcessStatement()
        {
            foreach (var statementType in Enum.GetValues<StatementType>())
            {
                if (ProcessStatementType(statementType))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ProcessStatementType(StatementType statement)
        {
            throw new NotImplementedException();
        }

        private bool ProcessForBlock()
        {
            throw new NotImplementedException();
        }
    }
}
