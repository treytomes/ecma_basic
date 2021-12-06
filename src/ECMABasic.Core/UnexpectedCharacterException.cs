using System;

namespace ECMABasic.Core
{
    public class UnexpectedCharacterException : Exception
    {
        public UnexpectedCharacterException(int line, int column, string expected, string found)
            : base($"({line}:{column}) Expected '{expected}', found '{found}'.")
        {
            Line = line;
            Column = column;
            Expected = expected;
            Found = found;
        }

        public UnexpectedCharacterException(int line, int column, string expected, char found)
            : this(line, column, expected, found.ToString())
        {
        }

        public int Line { get; }
        public int Column { get; }
        public string Expected { get; }
        public string Found { get; }
    }
}
