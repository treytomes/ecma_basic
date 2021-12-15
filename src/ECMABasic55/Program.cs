using ECMABasic.Core;
using System;
using System.IO;

namespace ECMABasic55
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ECMA Basic 55 Runtime Environment");

            if (args.Length < 1)
			{
                Console.WriteLine("Usage: ecmabasic55 {optional file path}");
                return;
			}

            var path = args[0];

            if (!File.Exists(path))
			{
                throw new FileNotFoundException(null, path);
			}

            var env = new ConsoleEnvironment();

            var interpreter = Interpreter.FromFile(path, env);
            var program = interpreter.Program;
            program.Execute(env);
        }
    }
}
