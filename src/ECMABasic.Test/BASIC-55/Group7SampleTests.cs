using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("FOR-NEXT")]
	public class Group7SampleTests : SampleTests
	{
		[Fact(DisplayName = "P044: Elementary use of the FOR-statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P044()
		{
			RunSample("P044");
		}

		[Fact(DisplayName = "P045: Altering the control-variable within a FOR-block.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P045()
		{
			RunSample("P045");
		}

		[Fact(DisplayName = "P046: Interaction of control statements with the FOR-statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P046()
		{
			RunSample("P046");
		}

		[Fact(DisplayName = "P047: Increment in the STEP clause of the FOR-statement defaults to a value of one.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P047()
		{
			RunSample("P047");
		}

		[Fact(DisplayName = "P048: Limit and increment in the FOR-statement are evaluated once upon entering the loop.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P048()
		{
			RunSample("P048");
		}

		[Fact(DisplayName = "P049: Nested FOR-blocks.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Standard Capabilities")]
		public void P049()
		{
			RunSample("P049");
		}

		[Fact(DisplayName = "P050: Error - FOR-statement without a matching NEXT-statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P050()
		{
			RunSample("P050");
		}

		[Fact(DisplayName = "P051: Error - NEXT-statement without a matching FOR-statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P051()
		{
			RunSample("P051");
		}

		[Fact(DisplayName = "P052: Error - Mismatched control-variables on FOR-statement and NEXT-statement.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P052()
		{
			RunSample("P052");
		}

		[Fact(DisplayName = "P053: Error - Interleaved FOR-blocks.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P053()
		{
			RunSample("P053");
		}

		[Fact(DisplayName = "P054: Error - Nested FOR-blocks with the same control variable.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P054()
		{
			RunSample("P054");
		}

		[Fact(DisplayName = "P055: Error - Jump into FOR-block.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P055()
		{
			RunSample("P055");
		}
	}
}
