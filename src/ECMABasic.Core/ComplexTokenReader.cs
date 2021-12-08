using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	/// <summary>
	/// Assembly simple tokens into more complex tokens, e.g. strings, keywords, and operators.
	/// </summary>
	/// <remarks>
	/// This is where the lexical analysis really happens.
	/// </remarks>
	public class ComplexTokenReader
	{
		/// <summary>
		/// The index into _tokens.
		/// </summary>
		private int _tokenIndex;
		private List<Token> _tokens;

		private ComplexTokenReader(SimpleTokenReader reader)
		{
			_tokens = new List<Token>();
			while (!reader.IsAtEnd)
			{
				_tokens.Add(reader.Next());
			}

			reader.Dispose();
		}

		public bool IsAtEnd => _tokenIndex >= _tokens.Count;

		public static ComplexTokenReader FromFile(string filename)
		{
			return new ComplexTokenReader(SimpleTokenReader.FromFile(filename));
		}

		public static ComplexTokenReader FromText(string text)
		{
			return new ComplexTokenReader(SimpleTokenReader.FromText(text));
		}

		/// <summary>
		/// Read an integer off of the token stream.
		/// </summary>
		/// <param name="maxDigits">If > 0, don't read any more digits than this.</param>
		/// <param name="throwOnError">If true, an exception will be thrown if an integer coule not be read.</param>
		/// <returns>The integer that was read.</returns>
		/// <exception cref="UnexpectedTokenException">Throws an exception if an integer could not be read.</exception>
		public int? NextInteger(int maxDigits = 0, bool throwOnError = true)
		{
			var value = 0;
			
			var token = Next(TokenType.Integer, throwOnError);
			if (token == null)
			{
				return null;
			}
			value = int.Parse(token.Text);

			if (maxDigits > 1)
			{
				// Read off maxDigits - 1 more digits, if they are available.
				for (var n = 1; n < maxDigits; n++)
				{
					token = Next(TokenType.Integer, false);
					if (token != null)
					{
						value = value * 10 + int.Parse(token.Text);
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				// Keep reading off digits as long as you can.
				while (true)
				{
					token = Next(TokenType.Integer, false);
					if (token != null)
					{
						value = value * 10 + int.Parse(token.Text);
					}
					else
					{
						break;
					}
				}
			}

			return value;
		}

		/// <summary>
		/// Get the next token off the stream, verifying the token type.
		/// A null token will be returned if the type doesn't match, or optionally an exception will be thrown instead.
		/// If a null token is returned, the reader will not be advanced to the next token.
		/// </summary>
		/// <param name="type">The type of token to retrieve.</param>
		/// <param name="throwOnError">If true, an exception will be thrown if the type does not match.</param>
		/// <returns>The token that was read.</returns>
		/// <exception cref="UnexpectedTokenException">Throws an exception if the token type doesn't match what was expected.</exception>
		public Token Next(TokenType type, bool throwOnError = true)
		{
			var startPosition = _tokenIndex;
			var token = Next();
			if ((token == null) || (token.Type != type))
			{
				if (throwOnError)
				{
					throw new UnexpectedTokenException(type, token);
				}
				else
				{
					_tokenIndex = startPosition;
					return null;
				}
			}
			else
			{
				return token;
			}
		}

		/// <summary>
		/// Pull the next token off of the stream.
		/// </summary>
		/// <remarks>
		/// No semantic analysis at this phase; that comes next.
		/// </remarks>
		/// <returns>The token that was read, or null at the end of the stream.</returns>
		public Token Next()
		{
			if (IsAtEnd)
			{
				return null;
			}

			var token = Read();
			if (token.Type == TokenType.Symbol)
			{
				if (token.Text == "\"")
				{
					return ReadRestOfString(token);
				}
				else if (token.Text == ",")
				{
					return new Token(TokenType.Comma, token);
				}
				else if (token.Text == ";")
				{
					return new Token(TokenType.Semicolon, token);
				}
				else if (token.Text == "(")
				{
					return new Token(TokenType.OpenParenthesis, token);
				}
				else if (token.Text == ")")
				{
					return new Token(TokenType.CloseParenthesis, token);
				}
				else if (token.Text == "=")
				{
					return new Token(TokenType.Equals, token);
				}
				else
				{
					return token;
				}
			}
			else if (token.Type == TokenType.Word)
			{
				if (token.Text == "END")
				{
					return new Token(TokenType.Keyword_END, token);
				}
				else if (token.Text == "LET")
				{
					return new Token(TokenType.Keyword_LET, token);
				}
				else if (token.Text == "PRINT")
				{
					return new Token(TokenType.Keyword_PRINT, token);
				}
				else if (token.Text == "STOP")
				{
					return new Token(TokenType.Keyword_STOP, token);
				}
				else if (token.Text == "TAB")
				{
					return new Token(TokenType.Keyword_TAB, token);
				}
				else if (token.Text.Length == 1)
				{
					var nextToken = Peek();
					if ((nextToken.Type == TokenType.Symbol) && (nextToken.Text == "$"))
					{
						Read();  // Read off the $.
						return new Token(TokenType.StringVariable, new[] { token, nextToken });
					}
					else
					{
						// TODO: Recognize other variable types.
						return token;
					}
				}
				else
				{
					return token;
				}
			}
			else
			{
				return token;
			}

			//throw new UnexpectedTokenException(token);
		}

		private Token ReadRestOfString(Token start)
		{
			List<Token> stringTokens = new List<Token>();
			stringTokens.Add(start);

			while (true)
			{
				var token = Read();
				stringTokens.Add(token);
				if (token.Type == TokenType.Symbol)
				{
					if (token.Text == "\"")
					{
						break;
					}
				}
			}

			return new Token(TokenType.String, stringTokens);
		}

		private Token Read()
		{
			if (IsAtEnd)
			{
				return null;
			}

			var token = _tokens[_tokenIndex];
			_tokenIndex++;
			return token;
		}

		private Token Peek()
		{
			if (IsAtEnd)
			{
				return null;
			}
			return _tokens[_tokenIndex];
		}
	}
}
