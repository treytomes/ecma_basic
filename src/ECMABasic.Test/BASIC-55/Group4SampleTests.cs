using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("Control Statements and REM")]
	public class Group4SampleTests : SampleTests
	{
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
	}
}
