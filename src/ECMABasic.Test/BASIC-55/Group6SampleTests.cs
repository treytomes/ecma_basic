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

		[Fact(DisplayName = "P028: Exception - Division by zero.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P028()
		{
			RunSample("P028");
		}

		[Fact(DisplayName = "P029: Exception - Overflow of numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P029()
		{
			RunSample("P029");
		}

		[Fact(DisplayName = "P030: Exception - Overflow of numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P030()
		{
			RunSample("P030");
		}

		[Fact(DisplayName = "P031: Exception - Zero raised to a negative power.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P031()
		{
			RunSample("P031");
		}

		[Fact(DisplayName = "P032: Exception - Negative quantity raised to a non-integral power.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P032()
		{
			RunSample("P032");
		}

		[Fact(DisplayName = "P033: Exception - Underflow of numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P033()
		{
			RunSample("P033");
		}

		[Fact(DisplayName = "P034: Exception - Underflow of numeric constants.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P034()
		{
			RunSample("P034");
		}

		[Fact(DisplayName = "P035: Exception - Overflow and underflow within sub-expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Exceptions")]
		public void P035()
		{
			RunSample("P035");
		}

		[Fact(DisplayName = "P036: Error - Unmatched parenthesis in numeric expressions.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P036()
		{
			RunSample("P036");
		}

		[Fact(DisplayName = "P037: Error - Use of '**' as operator.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P037()
		{
			RunSample("P037");
		}

		[Fact(DisplayName = "P038: Error - Use of adjacent operators.")]
		[Trait("Feature Set", "BASIC-55")]
		[Trait("Category", "Errors")]
		public void P038()
		{
			RunSample("P038");
		}
	}
}
