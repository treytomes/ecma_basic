using ECMABasic.Core;
using System;
using System.IO;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	public class SampleTests
	{
		protected static void RunSample(string sampleName)
		{
			var env = new TestEnvironment();

			var output = File.ReadAllText($"./Resources/{sampleName}.OK");
			output = NormalizeLineEndings(output);
			var expectedLines = output.Split(Environment.NewLine);

			if (Interpreter.FromFile($"./Resources/{sampleName}.BAS", env))
			{
				env.Program.Execute(env);
			}

			var actualLines = env.Text.Split(Environment.NewLine);

			for (var line = 0; line < expectedLines.Length; line++)
			{
				if (expectedLines[line] != actualLines[line])
				{
					for (var chn = 0; chn < expectedLines[line].Length; chn++)
					{
						if (expectedLines[line][chn] != actualLines[line][chn])
						{
							Assert.True(actualLines[line][chn] == expectedLines[line][chn], $"({line + 1}:{chn + 1}) Expected '{expectedLines[line][chn]}', found '{actualLines[line][chn]}'.");
						}
					}
				}
				else
				{
					Assert.Equal(expectedLines[line], actualLines[line]);
				}
			}
		}

		/// <summary>
		/// Replace all new-line characters with the currently assigned Environment.NewLine.
		/// </summary>
		/// <param name="text">The text to modify.</param>
		/// <returns>The modified text.</returns>
		private static string NormalizeLineEndings(string text)
		{
			if (text.Contains("\n"))
			{
				text = text.Replace("\r", string.Empty);
				text = text.Replace("\n", Environment.NewLine);
			}
			else if (text.Contains("\r"))
			{
				text = text.Replace("\n", string.Empty);
				text = text.Replace("\r", Environment.NewLine);
			}
			return text;
		}
	}
}
