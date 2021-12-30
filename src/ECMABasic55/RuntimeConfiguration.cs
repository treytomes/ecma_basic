using Microsoft.Extensions.Configuration;
using System;

namespace ECMABasic55
{
	public class RuntimeConfiguration
	{
		private const string DEFAULT_PREAMBLE = @"ECMA-55 MINIMAL BASIC RUNTIME ENVIRONMENT
USAGE: ECMABASIC55 {OPTIONAL FILE PATH}";
		private RuntimeConfiguration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
				.Build();

			Preamble = GetValueOrDefault(config, "preamble", DEFAULT_PREAMBLE);
		}

		public static RuntimeConfiguration Instance { get; } = new RuntimeConfiguration();

		public string Preamble { get; }

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
