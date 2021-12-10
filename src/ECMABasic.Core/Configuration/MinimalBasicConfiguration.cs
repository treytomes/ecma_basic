using Microsoft.Extensions.Configuration;
using System;

namespace ECMABasic.Core.Configuration
{
	/// <summary>
	/// Contains global configuration settings to control everything.
	/// </summary>
	public class MinimalBasicConfiguration : IBasicConfiguration
	{
		private MinimalBasicConfiguration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
				.Build();

			MaxLineLength = GetValueOrDefault(config, "maxLineLength", MinimalBasicConfigDefaults.MAX_LINE_LENGTH);
			MaxStringLength = GetValueOrDefault(config, "maxStringLength", MinimalBasicConfigDefaults.MAX_STRING_LENGTH);
			TerminalWidth = GetValueOrDefault(config, "terminalWidth", MinimalBasicConfigDefaults.TERMINAL_WIDTH);
			NumTerminalColumns = GetValueOrDefault(config, "numTerminalColumns", MinimalBasicConfigDefaults.NUM_TERMINAL_COLUMNS);
			MaxLineNumberDigits = GetValueOrDefault(config, "maxLineNumberDigits", MinimalBasicConfigDefaults.MAX_LINE_NUMBER_DIGITS);
		}

		public static IBasicConfiguration Instance { get; } = new MinimalBasicConfiguration();

		public int MaxLineLength { get; }

		public int MaxStringLength { get; }

		public int TerminalWidth { get; }

		public int NumTerminalColumns { get; }

		public int TerminalColumnWidth
		{
			get
			{
				return TerminalWidth / NumTerminalColumns;
			}
		}

		public int MaxLineNumberDigits { get; }

		private T GetValueOrDefault<T>(IConfiguration config, string key, T defaultValue)
		{
			var section = config.GetSection(key);
			var value = section.Value;
			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}
		}
	}
}
