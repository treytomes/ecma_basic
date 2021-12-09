using ECMABasic.Core;
using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	public class TokenizerTests
	{
		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Can_create_character_reader()
		{
			var sourceText = "Hello world!";
			using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(sourceText)))
			{
				using (var reader = new CharacterReader(stream))
				{
					Assert.Equal(sourceText, reader.SourceText);
					Assert.Equal(sourceText, reader.SourceText);
				}
			}
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Can_process_simple_tokens()
		{
			var inputText = "HELLO	 123 WORLD!?";
			using (var reader = SimpleTokenReader.FromText(inputText))
			{
				var tokens = new List<Token>();
				while (true)
				{
					var token = reader.Next();
					tokens.Add(token);
					if (token == null)
					{
						break;
					}
				}

				Assert.Equal(8, tokens.Count);
				
				Assert.Equal("HELLO", tokens[0].Text);
				Assert.Equal(TokenType.Word, tokens[0].Type);
				Assert.Equal(5, tokens[0].Length);
				Assert.Equal(1, tokens[0].Line);
				Assert.Equal(1, tokens[0].Column);

				Assert.Equal("	 ", tokens[1].Text);
				Assert.Equal(TokenType.Space, tokens[1].Type);
				Assert.Equal(2, tokens[1].Length);
				Assert.Equal(1, tokens[1].Line);
				Assert.Equal(6, tokens[1].Column);

				Assert.Equal("123", tokens[2].Text);
				Assert.Equal(TokenType.Integer, tokens[2].Type);
				Assert.Equal(3, tokens[2].Length);
				Assert.Equal(1, tokens[2].Line);
				Assert.Equal(8, tokens[2].Column);

				Assert.Equal(" ", tokens[3].Text);
				Assert.Equal(TokenType.Space, tokens[3].Type);
				Assert.Equal(1, tokens[3].Length);
				Assert.Equal(1, tokens[3].Line);
				Assert.Equal(11, tokens[3].Column);

				Assert.Equal("WORLD", tokens[4].Text);
				Assert.Equal(TokenType.Word, tokens[4].Type);
				Assert.Equal(5, tokens[4].Length);
				Assert.Equal(1, tokens[4].Line);
				Assert.Equal(12, tokens[4].Column);

				Assert.Equal("!", tokens[5].Text);
				Assert.Equal(TokenType.Symbol, tokens[5].Type);
				Assert.Equal(1, tokens[5].Length);
				Assert.Equal(1, tokens[5].Line);
				Assert.Equal(17, tokens[5].Column);


				Assert.Equal("?", tokens[6].Text);
				Assert.Equal(TokenType.Symbol, tokens[6].Type);
				Assert.Equal(1, tokens[6].Length);
				Assert.Equal(1, tokens[6].Line);
				Assert.Equal(18, tokens[6].Column);

				Assert.Null(tokens[7]);
			}
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Can_process_complex_tokens()
		{
			var input = @"10 PRINT ""HELLO WORLD!""
15 PRINT ""THIS IS A TEST...?""
20 END";
			var reader = ComplexTokenReader.FromText(input);

			// Line 10

			var token = reader.Next();
			Assert.Equal(TokenType.Integer, token.Type);
			Assert.Equal("10", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.Space, token.Type);

			token = reader.Next();
			Assert.Equal(TokenType.Keyword_PRINT, token.Type);
			Assert.Equal("PRINT", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.Space, token.Type);

			token = reader.Next();
			Assert.Equal(TokenType.String, token.Type);
			Assert.Equal(1, token.Line);
			Assert.Equal(10, token.Column);
			Assert.Equal("\"HELLO WORLD!\"", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.EndOfLine, token.Type);

			// Line 15

			token = reader.Next();
			Assert.Equal(TokenType.Integer, token.Type);
			Assert.Equal("15", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.Space, token.Type);

			token = reader.Next();
			Assert.Equal(TokenType.Keyword_PRINT, token.Type);
			Assert.Equal("PRINT", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.Space, token.Type);

			token = reader.Next();
			Assert.Equal(TokenType.String, token.Type);
			Assert.Equal("\"THIS IS A TEST...?\"", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.EndOfLine, token.Type);

			// Line 20

			token = reader.Next();
			Assert.Equal(TokenType.Integer, token.Type);
			Assert.Equal("20", token.Text);

			token = reader.Next();
			Assert.Equal(TokenType.Space, token.Type);

			token = reader.Next();
			Assert.Equal(TokenType.Keyword_END, token.Type);
			Assert.Equal("END", token.Text);

			token = reader.Next();
			Assert.Null(token);
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Long_lines_are_rejected()
		{
			var input = @"10 PRINT ""HELLO WORLD!""
20 PRINT ""0123456789012345678901234567890123456789012345678901234567890123456789""
30 END";
			using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(input)))
			{
				try
				{
					var reader = new CharacterReader(stream);
					throw new InvalidOperationException("This should have thrown an exception.");
				}
				catch (LineSyntaxException ex)
				{
					Assert.Equal("? LINE IS TOO LONG BY 9 CHARACTERS IN LINE 20", ex.Message);
				}
			}
		}
	}
}
