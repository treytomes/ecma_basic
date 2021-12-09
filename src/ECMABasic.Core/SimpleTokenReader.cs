using ECMABasic.Core.Exceptions;
using System;
using System.IO;
using System.Text;

namespace ECMABasic.Core
{
    /// <summary>
    /// Performance a simple processing of text into basic token classes.
    /// </summary>
    /// <remarks>
    /// Complex tokens, like negative floating-point numbers, will be handled later in the process.
    /// </remarks>
    public class SimpleTokenReader : IDisposable
    {
        private Stream _stream;
        private CharacterReader _reader;
        private bool _disposedValue;

        private SimpleTokenReader(Stream stream)
        {
            _stream = stream;
            _reader = new CharacterReader(stream);
        }

        public bool IsAtEnd => _reader.IsAtEnd;

        public static SimpleTokenReader FromFile(string filename)
        {
            return new SimpleTokenReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
        }
        
        public static SimpleTokenReader FromText(string text)
        {
            return new SimpleTokenReader(new MemoryStream(Encoding.ASCII.GetBytes(text)));
        }

        /// <summary>
        /// Pull the next token off of the stream.
        /// </summary>
        /// <returns>The token that was read, or null at the end of the stream.</returns>
        public Token Next()
        {
            if (IsAtEnd)
            {
                return null;
            }

            var line = _reader.LineNumber;
            var column = _reader.ColumnNumber;
            var ch = _reader.Peek();
            if (_reader.IsDigit(ch))
            {
                var text = _reader.ReadInteger();
                return new Token(TokenType.Digit, line, column, text);
            }
            else if (_reader.IsLetter(ch))
            {
                var text = _reader.ReadWord();
                return new Token(TokenType.Word, line, column, text);
            }
            else if (_reader.IsSymbol(ch))
            {
                var text = _reader.ReadSymbol();
                return new Token(TokenType.Symbol, line, column, text);
            }
            else if (_reader.IsSpace(ch))
            {
                var text = _reader.ReadSpace();
                return new Token(TokenType.Space, line, column, text);
            }
            else if (_reader.IsEndOfLine(ch))
			{
                var text = _reader.ReadEndOfLine();
                return new Token(TokenType.EndOfLine, line, column, text);
			}

            throw new UnexpectedCharacterException(line, column, "integer|word|symbol|space", ch);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reader.Dispose();
                    _stream.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
