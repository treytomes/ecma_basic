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

		[Fact(DisplayName = "P028: EXCEPTION - Division by zero.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P028()
		{
			RunSample("P028");
		}

		[Fact(DisplayName = "P029: EXCEPTION - Overflow of numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P029()
		{
			RunSample("P029");
		}

		[Fact(DisplayName = "P030: EXCEPTION - Overflow of numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P030()
		{
			RunSample("P030");
		}

		[Fact(DisplayName = "P031: EXCEPTION - Zero raised to a negative power.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P031()
		{
			RunSample("P031");
		}

		[Fact(DisplayName = "P032: EXCEPTION - Negative quantity raised to a non-integral power.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P032()
		{
			RunSample("P032");
		}

		[Fact(DisplayName = "P033: EXCEPTION - Underflow of numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P033()
		{
			RunSample("P033");
		}

		[Fact(DisplayName = "P034: EXCEPTION - Underflow of numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P034()
		{
			RunSample("P034");
		}

		[Fact(DisplayName = "P035:EXCEPTION - Overflow and underflow within sub-expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P035()
		{
			RunSample("P035");
		}
	}
}
