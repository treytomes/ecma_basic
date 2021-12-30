using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("PRINTing and simple assignment(\"LET\")")]
	public class Group3SampleTests : SampleTests
	{
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
	}
}
