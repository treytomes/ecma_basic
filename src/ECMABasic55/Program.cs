using ECMABasic.Core;
using System;
using System.IO;

namespace ECMABasic55
{
    // TODO: Implement SAVE.

    public static class Program
    {
        private static readonly IEnvironment _env = new ConsoleEnvironment();

        public static int Main(string[] args)
        {
            Console.WriteLine("ECMA Basic 55 Runtime Environment");
            Console.WriteLine("Usage: ecmabasic55 {optional file path}");
            Console.WriteLine();

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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(null, path);
            }

            Interpreter.FromFile(path, _env);
            _env.Program.Execute(_env);
            return 0;
        }

        private static int RunREPL()
		{
            var interpreter = new RuntimeInterpreter(_env);
            var isRunning = true;
            Console.WriteLine("OK");

            while (isRunning)
			{
                var line = Console.ReadLine() + Environment.NewLine;

				try
				{
                    var statement = interpreter.ProcessImmediate(line);
                    if (statement != null)
					{
                        statement.Execute(_env, true);
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
