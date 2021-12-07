namespace ECMABasic.Core
{
    /// <summary>
    /// The full list of statements built in to BASIC-55.
    /// </summary>
    public enum StatementType
    {
        /// <summary>
        /// Occurs once on the last line of the program.  Indicates the end of program execution.
        /// </summary>
        END,

        DATA,
        DEF,
        DIMENSION,
        GOSUB,
        GOTO,
        IF_THEN,
        INPUT,
        LET,
        ON_GOTO,
        OPTION,

        /// <summary>
        /// Write to the output stream.
        /// </summary>
        PRINT,

        RANDOMIZE,
        READ,
        REMARK,
        RESTORE,
        RETURN,
        
        /// <summary>
        /// Prematurely halt program execution.
        /// </summary>
        STOP
    }
}
