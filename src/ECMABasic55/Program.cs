using ECMABasic.Application;
using ECMABasic.Application.Configuration;
using ECMABasic.Domain;
using ECMABasic.Infrastructure;
using ECMABasic55.Logging;
using ECMABasic55.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

		root.SetHandler(async (file, config, debug) =>
		{
			var props = new CommandLineProps
			{
				FilePath = file,
				ConfigFile = config,
				Debug = debug
			};

			using var host = CreateHostBuilder(props).Build();
			var env = host.Services.GetRequiredService<IEnvironment>();
			var runtimeConfig = host.Services.GetRequiredService<RuntimeConfiguration>();

			var exitCode = props.FilePath != null
				? RunBatch(env, runtimeConfig, props.FilePath)
				: RunREPL(env, runtimeConfig);

			Environment.Exit(exitCode);
		}, fileArgument, configOption, debugOption);

		return root;
	}

	private static IHostBuilder CreateHostBuilder(CommandLineProps props)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration((ctx, config) => ConfigureAppConfiguration(config, props))
			.ConfigureLogging(ConfigureLogging)
			.ConfigureServices(ConfigureServices);
	}

	private static void ConfigureAppConfiguration(IConfigurationBuilder config, CommandLineProps props)
	{
		config.Sources.Clear();
		config.SetBasePath(AppContext.BaseDirectory);

		// Add YAML configuration file
		config.AddYamlFile(props.ConfigFile, optional: false, reloadOnChange: false);

		// Add command-line overrides
		var overrides = new Dictionary<string, string?>
		{
			["Debug"] = props.Debug.ToString()
		};
		config.AddInMemoryCollection(overrides);
	}

	private static void ConfigureLogging(HostBuilderContext ctx, ILoggingBuilder logging)
	{
		logging.ClearProviders();

		var debug = ctx.Configuration.GetValue<bool>("Debug");
		var minLevel = debug ? LogLevel.Debug : LogLevel.Information;

		// Add console logging in debug mode
		if (debug)
		{
			logging.AddConsole();
		}

		logging.SetMinimumLevel(minLevel);

		// Add file logging with daily rotation
		var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
		Directory.CreateDirectory(logDir);

		var logFile = Path.Combine(
			logDir,
			$"ecmabasic-{DateTime.UtcNow:yyyy-MM-dd}.log");

		logging.AddProvider(new FileLoggerProvider(logFile, minLevel));
	}

	private static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
	{
		// Bind configuration
		services.Configure<RuntimeConfiguration>(ctx.Configuration);
		services.AddSingleton<RuntimeConfiguration>(sp => sp.GetRequiredService<IOptions<RuntimeConfiguration>>().Value);

		// Register BASIC configuration - use existing singleton instance
		services.AddSingleton(MinimalBasicConfiguration.Instance);

		// Register interpreter and environment
		services.AddSingleton<Interpreter, RuntimeInterpreter>();
		services.AddSingleton<IEnvironment>(sp =>
		{
			var interpreter = (RuntimeInterpreter)sp.GetRequiredService<Interpreter>();
			var env = new ConsoleEnvironment(interpreter);
			InjectIntrinsics(env);
			return env;
		});
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

	private static int RunBatch(IEnvironment env, RuntimeConfiguration config, string path)
	{
		env.PrintLine(config.Preamble);
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

	private static int RunREPL(IEnvironment env, RuntimeConfiguration config)
	{
		env.PrintLine(config.Preamble);
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
