using ECMABasic.Core;
using System;
using System.ComponentModel;
using System.IO;
using Xunit;

namespace ECMABasic.Test.Basic55
{
	public class SampleTests
	{
		[Fact(DisplayName = "P001: Null print and printing quoted strings.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Simple PRINTing of string constants")]
		public void P001()
		{
			RunSample("P001");
		}

		[Fact(DisplayName = "P002: The END statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P002()
		{
			RunSample("P002");
		}

		[Fact(DisplayName = "P003: Error - Misplaced END statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P003()
		{
			RunSample("P003");
		}

		[Fact(DisplayName = "P004: Error - Missing END statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P004()
		{
			RunSample("P004");
		}

		[Fact(DisplayName = "P005: The STOP statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P005()
		{
			RunSample("P005");
		}

		[Fact(DisplayName = "P006: PRINT separators, TABs, and string variables.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P006()
		{
			RunSample("P006");
		}

		[Fact(DisplayName = "P007: Exception - String overflow using the LET statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P007()
		{
			RunSample("P007");
		}

		[Fact(DisplayName = "P008: Exception - TAB argument.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P008()
		{
			RunSample("P008");
		}

		[Fact(DisplayName = "P009: Printing NR1 and NR2 numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P009()
		{
			RunSample("P009");
		}

		[Fact(DisplayName = "P010: Printing NR3 numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P010()
		{
			RunSample("P010");
		}

		[Fact(DisplayName = "P011: Printing numeric variables assigned NR1 and NR2 constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P011()
		{
			RunSample("P011");
		}

		[Fact(DisplayName = "P012: Printing numeric variables assigned NR3 constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P012()
		{
			RunSample("P012");
		}

		[Fact(DisplayName = "P013: Format and rounding of printed numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P013()
		{
			RunSample("P013");
		}

		[Fact(DisplayName = "P014: Printing and assigning numeric values near to the maximum and minimum magnitude.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P014()
		{
			RunSample("P014");
		}

		[Fact(DisplayName = "P015: The REM and GOTO statements.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P015_The_REM_and_GOTO_statements()
		{
			RunSample("P015");
		}

		[Fact(DisplayName = "P016: Error - Transfer to a non existing line.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P016()
		{
			RunSample("P016");
		}

		[Fact(DisplayName = "P017: Elementary use of GOSUB and RETURN.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P017()
		{
			RunSample("P017");
		}

		[Fact(DisplayName = "P018: The IF-THEN statement with string operands.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P018()
		{
			RunSample("P018");
		}

		[Fact(DisplayName = "P019: The IF-THEN statement with numeric operands.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P019()
		{
			RunSample("P019");
		}

		[Fact(DisplayName = "P020: Error - IF-THEN statement with a string and numeric operand.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P020()
		{
			RunSample("P020");
		}

		[Fact(DisplayName = "P021: Error - Transfer to a non-existing line number using the IF-THEN statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P021()
		{
			RunSample("P021");
		}

		[Fact(DisplayName = "P022: Numeric and string variable names with the same initial letter.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Variables")]
		public void P022()
		{
			RunSample("P022");
		}

		[Fact(DisplayName = "P023: Initialization of string and numeric variables.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Variables")]
		public void P023()
		{
			RunSample("P023");
		}

		[Fact(DisplayName = "P024: Plus and minus.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P024()
		{
			RunSample("P024");
		}

		[Fact(DisplayName = "P025: Multiple, divide, and involute.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P025()
		{
			RunSample("P025");
		}

		[Fact(DisplayName = "P026: Dependence rules for numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P026()
		{
			RunSample("P026");
		}

		[Fact(DisplayName = "P027: Accuracy of constants and variables.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P027()
		{
			RunSample("P027");
		}

		private static void RunSample(string sampleName)
		{
			var env = new TestEnvironment();

			var output = File.ReadAllText($"./Resources/{sampleName}.OK");
			output = NormalizeLineEndings(output);
			var expectedLines = output.Split(Environment.NewLine);

			Interpreter.FromFile($"./Resources/{sampleName}.BAS", env);
			var program = env.Program;
			program.Execute(env);
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
