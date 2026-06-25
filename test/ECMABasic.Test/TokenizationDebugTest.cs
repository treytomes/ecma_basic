using System;
using System.Collections.Generic;
using ECMABasic.Application;
using ECMABasic.Domain;
using Xunit;
using Xunit.Abstractions;

namespace ECMABasic.Test;

/// <summary>
/// Debug test to understand how "DEF FND(X)" is being tokenized.
/// </summary>
public class TokenizationDebugTest
{
	private readonly ITestOutputHelper _output;

	public TokenizationDebugTest(ITestOutputHelper output)
	{
		_output = output;
	}

	[Fact]
	public void Debug_Tokenize_DEF_FND()
	{
		var text = "20 DEF FND(X)=P*X/180\n";
		var reader = ComplexTokenReader.FromText(text);

		var tokens = new List<string>();
		while (!reader.IsAtEnd)
		{
			var token = reader.Next();
			if (token != null)
			{
				tokens.Add($"{token.Type}:'{token.Text}'");
			}
		}

		// Print all tokens
		_output.WriteLine("Tokens for: " + text.Trim());
		foreach (var token in tokens)
		{
			_output.WriteLine("  " + token);
		}

		// This test always passes - it's just for debugging output
		Assert.True(true);
	}

	[Fact]
	public void Debug_Tokenize_DEF_FN_D()
	{
		var text = "20 DEF FN D(X)=P*X/180\n";
		var reader = ComplexTokenReader.FromText(text);

		var tokens = new List<string>();
		while (!reader.IsAtEnd)
		{
			var token = reader.Next();
			if (token != null)
			{
				tokens.Add($"{token.Type}:'{token.Text}'");
			}
		}

		// Print all tokens
		_output.WriteLine("Tokens for: " + text.Trim());
		foreach (var token in tokens)
		{
			_output.WriteLine("  " + token);
		}

		// This test always passes - it's just for debugging output
		Assert.True(true);
	}
}
