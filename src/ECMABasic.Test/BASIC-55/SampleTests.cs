using ECMABasic.Core;
using System;
using System.IO;
using Xunit;

namespace ECMABasic.Test.Basic55
{
	public class SampleTests
	{
		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Simple PRINTing of string constants")]
		public void P001_Null_print_and_printing_quoted_strings()
		{
			RunSample("P001");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P002_The_END_statement()
		{
			RunSample("P002");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P003_Error_Misplaced_END_statement()
		{
			RunSample("P003");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P004_Error_Missing_END_statement()
		{
			RunSample("P004");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "END and STOP")]
		public void P005_The_STOP_statement()
		{
			RunSample("P005");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P006_PRINT_separators_TABs_and_string_variables()
		{
			RunSample("P006");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P007_Exception_String_overflow_using_the_LET_statement()
		{
			RunSample("P007");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "String variables and TAB")]
		public void P008_Exception_TAB_argument()
		{
			RunSample("P008");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P009_Printing_NR1_and_NR2_numeric_constants()
		{
			RunSample("P009");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P010_Printing_NR3_numeric_constants()
		{
			RunSample("P010");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P011_Printing_numeric_Variables_assigned_NR1_and_NR2_constants()
		{
			RunSample("P011");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P012_Printing_numeric_variables_assigned_NR3_constants()
		{
			RunSample("P012");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P013_Format_and_rounding_of_printed_numeric_constants()
		{
			RunSample("P013");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Numeric constants and variables")]
		public void P014_Printing_and_assigning_numeric_values_near_to_the_maximum_and_minimum_magnitude()
		{
			RunSample("P014");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P015_The_REM_and_GOTO_statements()
		{
			RunSample("P015");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P016_ERROR_Transfer_to_a_non_existing_line()
		{
			RunSample("P016");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P017_Elementary_use_of_GOSUB_and_RETURN()
		{
			RunSample("P017");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P018_The_IF_THEN_statement_with_string_operands()
		{
			RunSample("P018");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P019_The_IF_THEN_statement_with_numeric_operands()
		{
			RunSample("P019");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P020_ERROR_IF_THEN_statement_with_a_string_and_numeric_operand()
		{
			RunSample("P020");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Control Statements and REM")]
		public void P021_ERROR_Transfer_to_a_non_existing_line_number_using_the_IF_THEN_statement()
		{
			RunSample("P021");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Variables")]
		public void P022_Numeric_and_string_variable_names_with_the_same_initial_letter()
		{
			RunSample("P022");
		}

		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Variables")]
		public void P023_Initialization_of_string_and_numeric_variables()
		{
			RunSample("P023");
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
