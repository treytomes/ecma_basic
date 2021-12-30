using ECMABasic.Core;
using ECMABasic.Core.Statements;
using System;
using System.Linq;
using Xunit;

namespace ECMABasic.Test.BASIC_55
{
	public class InterpreterTests
	{
		[Fact]
		[Trait("Feature Set", "BASIC-55")]
		public void Can_interpret_PRINT()
		{
			var env = new TestEnvironment();

			var sourceText = @"10 PRINT ""HELLO, WORLD!""
20 END";
			Interpreter.FromText(sourceText, env);
			var program = env.Program;

			Assert.Equal(2, program.Length);

			var line = program[10];
			Assert.NotNull(line);
			Assert.Equal(10, line.LineNumber);
			Assert.IsType<PrintStatement>(line.Statement);
			Assert.Equal("HELLO, WORLD!", (line.Statement as PrintStatement).PrintItems.First().Evaluate(env));

			line = program[20];
			Assert.NotNull(line);
			Assert.Equal(20, line.LineNumber);
			Assert.IsType<EndStatement>(line.Statement);

			program.Execute(env);

			Assert.Equal("HELLO, WORLD!" + Environment.NewLine, env.Text);
		}
	}
}
