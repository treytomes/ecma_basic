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
            var output = File.ReadAllText("./Resources/P001.OK");

            var interpreter = Interpreter.FromFile("./Resources/P001.BAS");
            var program = interpreter.Program;
            var env = new TestEnvironment();
            program.Execute(env);

            Assert.Equal(output, env.Text);
        }

        [Fact]
        [Trait("Feature Set", "BASIC-55")]
        [Trait("Category", "Simple_PRINTing_of_string_constants")]
        public void The_END_statement()
		{
            var output = File.ReadAllText("./Resources/P002.OK");

            var interpreter = Interpreter.FromFile("./Resources/P002.BAS");
            var program = interpreter.Program;
            var env = new TestEnvironment();
            program.Execute(env);

            Assert.Equal(output, env.Text);
        }
    }
}
