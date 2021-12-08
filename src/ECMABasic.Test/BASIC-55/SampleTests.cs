using ECMABasic.Core;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace ECMABasic.Test.Basic55
{
    public class SampleTests
    {
        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "Simple PRINTing of string constants")]
        public void Null_print_and_printing_quoted_strings()
        {
            RunSample("P001");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "END and STOP")]
        public void The_END_statement()
        {
            RunSample("P002");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "END and STOP")]
        public void Error_Misplaced_END_statement()
        {
            RunSample("P003");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "END and STOP")]
        public void Error_Missing_END_statement()
        {
            RunSample("P004");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "END and STOP")]
        public void The_STOP_statement()
        {
            RunSample("P005");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "String variables and TAB")]
        public void PRINT_separators_TABs_and_string_variables()
        {
            RunSample("P006");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "String variables and TAB")]
        public void Exception_String_overflow_using_the_LET_statement()
        {
            // TODO: This shouldn't be passing yet...
            RunSample("P007");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "String variables and TAB")]
        public void Exception_TAB_argument()
        {
            RunSample("P008");
        }

        private void RunSample(string sampleName)
		{
            var env = new TestEnvironment();

            var output = File.ReadAllText($"./Resources/{sampleName}.OK");
            var interpreter = Interpreter.FromFile($"./Resources/{sampleName}.BAS", env);
            var program = interpreter.Program;
            program.Execute(env);
            Assert.Equal(output, env.Text);
        }
    }
}
