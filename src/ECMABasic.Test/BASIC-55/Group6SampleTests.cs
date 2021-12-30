using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("Numeric Constants, Variables, and Operations")]
	public class Group6SampleTests : SampleTests
	{
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
	}
}
