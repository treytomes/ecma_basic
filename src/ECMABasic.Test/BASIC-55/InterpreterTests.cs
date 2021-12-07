using ECMABasic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	public class InterpreterTests
	{
		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Can_interpret_PRINT()
		{
			var sourceText = @"10 PRINT ""HELLO, WORLD!""
20 END";
			var interpreter = Interpreter.FromText(sourceText);
			var program = interpreter.Program;

			Assert.Equal(2, program.Length);

			var line = program[10];
			Assert.NotNull(line);
			Assert.Equal(10, line.LineNumber);
			Assert.IsType<PrintStatement>(line.Statement);
			Assert.Equal("HELLO, WORLD!", (line.Statement as PrintStatement).Expression.ToString());

			line = program[20];
			Assert.NotNull(line);
			Assert.Equal(20, line.LineNumber);
			Assert.IsType<EndStatement>(line.Statement);

			var env = new TestEnvironment();
			program.Execute(env);

			Assert.Equal("HELLO, WORLD!\r\n", env.Text);
		}
	}
}
