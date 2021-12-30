using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("END and STOP")]
	public class Group2SampleTests : SampleTests
	{
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
	}
}
