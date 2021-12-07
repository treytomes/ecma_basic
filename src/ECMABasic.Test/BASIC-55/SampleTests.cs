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
        [Trait("Category", "Simple_PRINTing_of_string_constants")]
        public void Null_print_and_printing_quoted_strings()
        {
            RunSample("P001");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "Simple_PRINTing_of_string_constants")]
        public void The_END_statement()
        {
            RunSample("P002");
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "Simple_PRINTing_of_string_constants")]
        public void Error_Misplaced_END_statement()
        {
            RunSample("P003");
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
