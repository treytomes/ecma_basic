using ECMABasic.Application;
using ECMABasic.Domain;
using ECMABasic.Infrastructure;
using ECMABasic55.Parsers;
using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;

namespace ECMABasic55;

// TODO: Better error checking around INPUT.
// TODO: DATA string datums shouldn't require quotes.

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		return await BuildCommandLine().InvokeAsync(args);
	}

	private static RootCommand BuildCommandLine()
	{
		var fileArgument = new Argument<string?>(
			name: "file",
			description: "BASIC program file to execute in batch mode",
			getDefaultValue: () => null);

		var configOption = new Option<string>(
			name: "--config",
			description: "Path to the configuration file",
			getDefaultValue: () => "appsettings.yaml");

		var debugOption = new Option<bool>(
			name: "--debug",
			description: "Enable debug logging");

		var root = new RootCommand("ECMA-55 BASIC Interpreter");
		root.AddArgument(fileArgument);
		root.AddOption(configOption);
		root.AddOption(debugOption);

		root.SetHandler((file, config, debug) =>
		{
			// TODO: Use config and debug parameters in future DI setup
			_ = config;
			_ = debug;

			var exitCode = file != null ? RunBatch(file) : RunREPL();
			Environment.Exit(exitCode);
		}, fileArgument, configOption, debugOption);

		return root;
	}

	private static void InjectIntrinsics(IEnvironment env)
	{
		((EnvironmentBase)env).Interpreter.InjectStatements([
			new SleepStatementParser(),
		]);

		FunctionFactory.Instance.Define("ASC", [ExpressionType.String], args => (int)args[0].ToString()![0]);

		FunctionFactory.Instance.Define("MID$", [ExpressionType.String, ExpressionType.Number, ExpressionType.Number], args =>
		{
			var source = Convert.ToString(args[0]) ?? string.Empty;
			var startIndex = Convert.ToInt32(args[1]);
			var length = Convert.ToInt32(args[2]);
			return source.Substring(startIndex - 1, length);
		});

		FunctionFactory.Instance.Define("MID$", [ExpressionType.String, ExpressionType.Number], args =>
		{
			var source = Convert.ToString(args[0]) ?? string.Empty;
			var startIndex = Convert.ToInt32(args[1]);
			return source[(startIndex - 1)..];
		});

		FunctionFactory.Instance.Define("POS", [ExpressionType.String, ExpressionType.String, ExpressionType.Number], args =>
		{
			var source = Convert.ToString(args[0]) ?? string.Empty;
			var value = Convert.ToString(args[1]) ?? string.Empty;
			var index = Convert.ToInt32(args[2]);
			return (double)source.IndexOf(value, index - 1) + 1;
		});

		FunctionFactory.Instance.Define("POS", [ExpressionType.String, ExpressionType.String], args =>
		{
			var source = Convert.ToString(args[0]) ?? string.Empty;
			var value = Convert.ToString(args[1]) ?? string.Empty;
			return (double)source.IndexOf(value) + 1;
		});
	}

	private static int RunBatch(string path)
	{
		IEnvironment env = new ConsoleEnvironment();
		InjectIntrinsics(env);
		env.PrintLine(RuntimeConfiguration.Instance.Preamble);
		env.PrintLine();

		if (!File.Exists(path))
		{
			throw new FileNotFoundException(null, path);
		}

		if (env.LoadFile(path))
		{
			((EnvironmentBase)env).Program.Execute(env);
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
		InjectIntrinsics(env);
		env.PrintLine(RuntimeConfiguration.Instance.Preamble);
		env.PrintLine();

		var isRunning = true;
		Console.WriteLine("OK");

		while (isRunning)
		{
			var line = (Console.ReadLine() ?? string.Empty) + Environment.NewLine;

			try
			{
				var statement = (((EnvironmentBase)env).Interpreter as RuntimeInterpreter)?.ProcessImmediate(env, line);
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
