using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("Variables")]
	public class Group5SampleTests : SampleTests
	{
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
	}
}
