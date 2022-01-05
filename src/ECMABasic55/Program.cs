﻿using ECMABasic.Core;
using ECMABasic55.Parsers;
using System;
using System.IO;

namespace ECMABasic55
{
    public static class Program
    {
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

        private static void InjectSpecials(IEnvironment env)
		{
            env.Interpreter.InjectStatements(new[] {
                new SleepStatementParser(),
            });

            FunctionFactory.Instance.Define("ASC", new[] { ExpressionType.String }, args => (int)args[0].ToString()[0]);
        }

        private static int RunBatch(string path)
		{
			IEnvironment env = new ConsoleEnvironment();
            InjectSpecials(env);
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
            InjectSpecials(env);
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
