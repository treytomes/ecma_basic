using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECMABasic.Core
{
    public class Token
    {
        /// <summary>
        /// Create a token from an aggregate of several other tokens.
        /// </summary>
        /// <param name="type">The type of the new token.</param>
        /// <param name="tokens">The tokens to combine into a single token.</param>
        public Token(TokenType type, IEnumerable<Token> tokens)
		{
            Type = type;
            Line = tokens.First().Line;
            Column = tokens.First().Column;

            var sb = new StringBuilder();
            foreach (var token in tokens)
			{
                sb.Append(token.Text);
			}
            Text = sb.ToString();
		}

        /// <summary>
        /// Clone an existing token, optionally modifying it's type.
        /// </summary>
        /// <param name="type">The type of the new token.</param>
        /// <param name="token">The token to clone.</param>
        public Token(TokenType type, Token token)
		{
            Type = type;
            Line = token.Line;
            Column = token.Column;
            Text = token.Text;
		}

        /// <summary>
        /// Create a token.
        /// </summary>
        /// <param name="type">The type of the new token.</param>
        /// <param name="line">The line number in the source text that this token starts on.</param>
        /// <param name="column">The column number in the source text that this token ends on.</param>
        /// <param name="text">The full text of the token.</param>
        public Token(TokenType type, int line, int column, string text)
        {
            Type = type;
            Line = line;
            Column = column;
            Text = text;
        }

        /// <summary>
        /// What type of token is this?
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The line number in the source text that this token starts on.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The column number in the source text that this token ends on.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// The full text of the token.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The full length of this token's text.
        /// </summary>
        public int Length => Text.Length;

		public override string ToString()
		{
            return $"({Line}:{Column}) {Text}";
		}
	}
}
