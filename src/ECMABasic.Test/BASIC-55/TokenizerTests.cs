using ECMABasic.Core;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	public class TokenizerTests
	{
		[Fact]
		[Trait("BASIC-55", "Tokenizer")]
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
		[Trait("BASIC-55", "Tokenizer")]
		public void Can_process_simple_tokens()
		{
			var sourceText = "HELLO	 123 WORLD!?";
			using (var reader = SimpleTokenReader.FromText(sourceText))
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

				Assert.Equal(7, tokens.Count);
				
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

				Assert.Equal("!?", tokens[5].Text);
				Assert.Equal(TokenType.Symbol, tokens[5].Type);
				Assert.Equal(2, tokens[5].Length);
				Assert.Equal(1, tokens[5].Line);
				Assert.Equal(17, tokens[5].Column);

				Assert.Null(tokens[6]);
			}
		}
	}
}
