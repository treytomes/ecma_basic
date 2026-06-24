using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ECMABasic55;

public class RuntimeConfiguration
{
	private const string _defaultPreamble = @"ECMA-55 MINIMAL BASIC RUNTIME ENVIRONMENT
USAGE: ECMABASIC55 {OPTIONAL FILE PATH}";
	private RuntimeConfiguration()
	{
		var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.yaml");
		var yamlContent = File.ReadAllText(configPath);
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();
		var config = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

		Preamble = GetValueOrDefault(config, "preamble", _defaultPreamble);
	}

	public static RuntimeConfiguration Instance { get; } = new RuntimeConfiguration();

	public string Preamble { get; }

	private T GetValueOrDefault<T>(Dictionary<string, object> config, string key, T defaultValue)
	{
		if (!config.TryGetValue(key, out var value) || value == null)
		{
			return defaultValue;
		}

		return (T)Convert.ChangeType(value, typeof(T));
	}
}
