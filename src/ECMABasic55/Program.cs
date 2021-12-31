using ECMABasic.Core;
using ECMABasic55.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ECMABasic55
{
    public static class Program
    {
        private static readonly List<StatementParser> _additionalStatements = new()
        {
            new SleepStatementParser(),
        };

        public static int Main(string[] args)
        {
            if (args.Length == 1)
			{
                return RunBatch(args[0]);
			}
            else
			{
                return RunREPL();
			}
        }

        private static int RunBatch(string path)
		{
			IEnvironment env = new ConsoleEnvironment();
            env.Interpreter.InjectStatements(_additionalStatements);
            env.PrintLine(RuntimeConfiguration.Instance.Preamble);
            env.PrintLine();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(null, path);
            }

            if (env.LoadFile(path))
            {
                env.Program.Execute(env);
                return 0;
            }
            else
			{
                return -1;
			}
        }

        private static int RunREPL()
		{
            IEnvironment env = new ConsoleEnvironment(new RuntimeInterpreter());
            env.Interpreter.InjectStatements(_additionalStatements);
            env.PrintLine(RuntimeConfiguration.Instance.Preamble);
            env.PrintLine();

            var isRunning = true;
            Console.WriteLine("OK");

            while (isRunning)
			{
                var line = Console.ReadLine() + Environment.NewLine;

				try
				{
                    var statement = (env.Interpreter as RuntimeInterpreter).ProcessImmediate(env, line);
                    if (statement != null)
					{
                        statement.Execute(env, true);
                        Console.WriteLine();
                        Console.WriteLine("OK");
                    }
                }
				catch (Exception ex)
				{
                    Console.WriteLine(ex.Message);
				}
			}

			return 0;
		}
    }
}
