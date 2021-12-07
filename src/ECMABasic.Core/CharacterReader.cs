using System;
using System.IO;
using System.Text;

namespace ECMABasic.Core
{
    /// <summary>
    /// Read a sequence of characters from a text source.
    /// </summary>
    /// <remarks>
    /// The source stream is constructed outside of this class's scope; it will also need to be disposed of outside of this class's scope.
    /// </remarks>
    public class CharacterReader : IDisposable
    {
        #region Constants

        private const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DIGITS = "0123456789";
        private const string SYMBOLS = "!\"%&'()*+,-./:;<=>?_#$#@[\\]^`{|}~";
        private const string SPACES = " \t";
        private const char CARRIAGE_RETURN = (char)13;
        private const char LINE_FEED = (char)10;

        #endregion

        #region Fields

        private Stream _source;
        private TextReader _reader;
        private bool _disposedValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="CharacterReader"/>.
        /// </summary>
        /// <param name="source">The source to pull the token text from.</param>
        public CharacterReader(Stream source)
        {
            _source = source;
            _reader = new StreamReader(_source, leaveOpen: true);
            LineNumber = 1;
            ColumnNumber = 1;
        }

        #endregion

        #region Properties

        public string SourceText
        {
            get
            {
                var position = _source.Position;
                _source.Position = 0;
                using (var reader = new StreamReader(_source, leaveOpen: true))
                {
                    var text = reader.ReadToEnd();
                    _source.Position = position;
                    return text;
                }
            }
        }
        public int LineNumber { get; private set; }
        public int ColumnNumber { get; private set; }
        public bool IsAtEnd => _reader.Peek() == -1;

        #endregion

        #region Methods

        public string ReadInteger()
        {
            var sb = new StringBuilder();
            sb.Append(RequireDigit());
            while (true)
            {
                var ch = Peek();
                if (IsDigit(ch))
                {
                    sb.Append(ch);
                    Next();
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public string ReadWord()
        {
            var sb = new StringBuilder();
            sb.Append(RequireLetter());
            while (true)
            {
                var ch = Peek();
                if (IsLetter(ch))
                {
                    sb.Append(ch);
                    Next();
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public string ReadSymbol()
        {
            var sb = new StringBuilder();
            sb.Append(RequireSymbol());
            return sb.ToString();
        }

        public string ReadSpace()
        {
            var sb = new StringBuilder();
            sb.Append(RequireSpace());
            while (true)
            {
                var ch = Peek();
                if (IsSpace(ch))
                {
                    sb.Append(ch);
                    Next();
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public string ReadEndOfLine()
		{
            var sb = new StringBuilder();

            var eol = RequireEndOfLine();
            sb.Append(eol);

            // The line-feed character is optional, but should still be recorded.
            var ch = Peek();
            if (((eol == CARRIAGE_RETURN) && (ch == LINE_FEED)) || ((eol == LINE_FEED) && (ch == CARRIAGE_RETURN)))
            {
                sb.Append(ch);
                Next();
            }

            return sb.ToString();
		}

        /// <summary>
        /// Require that a single letter be next in the token stream.
        /// Move to the next byte, and throw an exception if the token doesn't match.
        /// </summary>
        /// <returns>
        /// The letter that was read.
        /// </returns>
        public char RequireLetter()
        {
            var ch = Peek();
            if (!IsLetter(ch))
            {
                throw new UnexpectedCharacterException(LineNumber, ColumnNumber, LETTERS, ch);
            }
            Next();
            return ch;
        }

        /// <summary>
        /// Require that a single digit be next in the token stream.
        /// Move to the next byte, and throw an exception if the token doesn't match.
        /// </summary>
        /// <returns>
        /// The digit that was read.
        /// </returns>
        public char RequireDigit()
        {
            var ch = Peek();
            if (!IsDigit(ch))
            {
                throw new UnexpectedCharacterException(LineNumber, ColumnNumber, LETTERS, ch);
            }
            Next();
            return ch;
        }

        /// <summary>
        /// Require that a single symbol be next in the token stream.
        /// Move to the next byte, and throw an exception if the token doesn't match.
        /// </summary>
        /// <returns>
        /// The symbol that was read.
        /// </returns>
        public char RequireSymbol()
        {
            var ch = Peek();
            if (!IsSymbol(ch))
            {
                throw new UnexpectedCharacterException(LineNumber, ColumnNumber, SYMBOLS, ch);
            }
            Next();
            return ch;
        }

        /// <summary>
        /// Require that a single space be next in the token stream.
        /// Move to the next byte, and throw an exception if the token doesn't match.
        /// </summary>
        /// <returns>
        /// The space that was read.
        /// </returns>
        public char RequireSpace()
        {
            var ch = Peek();
            if (!IsSpace(ch))
            {
                throw new UnexpectedCharacterException(LineNumber, ColumnNumber, SPACES, ch);
            }
            Next();
            return ch;
        }

        public char RequireEndOfLine()
        {
            var ch = Peek();
            if ((ch != CARRIAGE_RETURN) && (ch != LINE_FEED))
            {
                throw new UnexpectedCharacterException(LineNumber, ColumnNumber, "end-of-line", ch);
            }
            Next();
            return ch;
        }

        public bool IsLetter(char ch)
        {
            return LETTERS.Contains(ch);
        }

        public bool IsDigit(char ch)
        {
            return DIGITS.Contains(ch);
        }

        public bool IsSpace(char ch)
        {
            return SPACES.Contains(ch);
        }

        public bool IsSymbol(char ch)
        {
            return SYMBOLS.Contains(ch);
        }

        public bool IsEndOfLine(char ch)
        {
            return (ch == CARRIAGE_RETURN) || (ch == LINE_FEED);
        }

        public char Peek()
        {
            return (char)_reader.Peek();
        }

        public char Next()
        {
            var ch = (char)_reader.Read();
            ColumnNumber++;

            if (ch == CARRIAGE_RETURN)
			{
                LineNumber++;
                ColumnNumber = 1;
			}

            return ch;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reader.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
