using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ECMABasic.Core
{
	// TODO: Do I *really* need to tokenizers?

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
		private readonly List<Token> _tokens;

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
			var signToken = Next(TokenType.Symbol, false, @"\+");
			if (signToken == null)
			{
				signToken = Next(TokenType.Symbol, false, @"\-");
			}
			var isNegative = signToken?.Text == "-";

			var token = Next(TokenType.Integer, throwOnError);
			if (token == null)
			{
				return null;
			}

			if ((maxDigits > 1) && (token.Text.Length > maxDigits))
			{
				throw new SyntaxException($"({token.Line}:{token.Column}) Integer is too long.");
			}

			int value = int.Parse(token.Text);
			if (isNegative)
			{
				value = -value;
			}
			return value;
		}

		/// <summary>
		/// Read a number off of the token stream.  The number can have a decimal point.
		/// </summary>
		/// <param name="throwOnError">If true, an exception will be thrown if an integer coule not be read.</param>
		/// <returns>The number that was read.</returns>
		/// <exception cref="UnexpectedTokenException">Throws an exception if an integer could not be read.</exception>
		public double? NextNumber(bool throwOnError = true)
		{
			var signToken = Next(TokenType.Symbol, false, @"\+");
			if (signToken == null)
			{
				signToken = Next(TokenType.Symbol, false, @"\-");
			}

			var integerToken = Next(TokenType.Integer, false);
			var decimalToken = Next(TokenType.DecimalPoint, false);
			string valueText;
			if ((decimalToken == null) && (integerToken != null))
			{
				valueText = string.Concat(signToken?.Text ?? string.Empty, integerToken.Text, ".0");
			}
			else
			{
				var fractionToken = Next(TokenType.Integer, throwOnError);
				if ((fractionToken == null) && (integerToken == null))
				{
					return null;
				}

				valueText = string.Concat(signToken?.Text ?? string.Empty, integerToken?.Text ?? string.Empty, ".", fractionToken?.Text ?? string.Empty);
			}

			// TODO: I don't really like how the "E" is typed here.
			var exradToken = Next(TokenType.Word, false, @"E\d?");
			if (exradToken != null)
			{
				var signText = string.Empty;
				if (exradToken.Text == "E")
				{
					var exsignToken = Next(TokenType.Symbol, false, @"\+");
					if (exsignToken == null)
					{
						exsignToken = Next(TokenType.Symbol, false, @"\-");
					}
					signText = exsignToken?.Text ?? string.Empty;
				}

				Token powerToken = null;
				if (!char.IsDigit(exradToken.Text.Last()))
				{
					powerToken = Next(TokenType.Integer, true);
				}

				valueText = string.Concat(valueText, exradToken.Text, signText, powerToken?.Text ?? string.Empty);
			}

			try
			{
				return double.Parse(valueText, System.Globalization.NumberStyles.Float);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException($"'{valueText}' couldn't be converted.", ex);
			}
		}

		/// <summary>
		/// Get the next token off the stream, verifying the token type.
		/// A null token will be returned if the type doesn't match, or optionally an exception will be thrown instead.
		/// If a null token is returned, the reader will not be advanced to the next token.
		/// </summary>
		/// <param name="type">The type of token to retrieve.</param>
		/// <param name="throwOnError">If true, an exception will be thrown if the type does not match.</param>
		/// <param name="pattern">An optional regular expression pattern to match on the next token text.</param>
		/// <returns>The token that was read.</returns>
		/// <exception cref="UnexpectedTokenException">Throws an exception if the token type doesn't match what was expected.</exception>
		public Token Next(TokenType type, bool throwOnError = true, string pattern = null)
		{
			var startPosition = _tokenIndex;
			var token = Next();
			if ((token == null) || (token.Type != type) || ((pattern != null) && !Regex.IsMatch(token.Text, @"^" + pattern + @"$", RegexOptions.Singleline)))
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
					var stringToken = ReadRestOfString(token);
					if (stringToken != null)
					{
						return stringToken;
					}
					else
					{
						return token;
					}
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
				else if (token.Text == ".")
				{
					return new Token(TokenType.DecimalPoint, token);
				}
				else
				{
					return token;
				}
			}
			else if (token.Type == TokenType.Word)
			{
				if (token.Text.Length == 1)
				{
					var nextToken = Peek();
					if ((nextToken.Type == TokenType.Symbol) && (nextToken.Text == "$"))
					{
						Read();  // Read off the $.
						return new Token(TokenType.Word, new[] { token, nextToken });
					}
					else
					{
						nextToken = Peek();
						if ((nextToken.Type == TokenType.Integer) && (nextToken.Text.Length == 1))
						{
							Read();  // Read off the digit.
							// It's a numeric variable with a letter followed by a digit.
							return new Token(TokenType.Word, new[] { token, nextToken });
						}
						else
						{
							// It's a numeric variable with just a single letter.
							return new Token(TokenType.Word, new[] { token });
						}
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
			var startIndex = _tokenIndex;
			List<Token> stringTokens = new() { start };

			while (true)
			{
				var token = Read();
				if ((token == null) || (token.Type == TokenType.EndOfLine))
				{
					// Rewind.
					_tokenIndex = startIndex;
					return null;
				}

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

		public Token Peek()
		{
			if (IsAtEnd)
			{
				return null;
			}
			return _tokens[_tokenIndex];
		}

		public void Rewind()
		{
			if (_tokenIndex == 0)
			{
				return;
			}
			_tokenIndex--;
		}
	}
}
