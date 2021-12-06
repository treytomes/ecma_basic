namespace ECMABasic.Core
{
    public class Token
    {
        public Token(TokenType type, int line, int column, string text)
        {
            Type = type;
            Line = line;
            Column = column;
            Text = text;
        }

        public TokenType Type { get; }
        public int Line { get; }
        public int Column { get; }
        public string Text { get; }
        public int Length => Text.Length;
    }
}
