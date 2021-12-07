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
        [Trait("BASIC-55", "Sample")]
        public void Simple_PRINTing_of_string_constants()
        {
            var output = File.ReadAllText("./Resources/P001.OK");

            var interpreter = Interpreter.FromFile("./Resources/P001.BAS");
            var program = interpreter.Program;
            var env = new TestEnvironment();
            program.Execute(env);

            Assert.Equal(output, env.Text);
        }
    }
}
