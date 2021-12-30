using System.ComponentModel;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	[Description("Simple PRINTing of string constants")]
	public class Group1SampleTests : SampleTests
	{
		[Fact(DisplayName = "P001: Null print and printing quoted strings.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Simple PRINTing of string constants")]
		public void P001()
		{
			RunSample("P001");
		}
	}
}
