using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ECMABasic55;

public static class YamlConfigurationExtensions
{
#pragma warning disable IDE0060 // Remove unused parameter - matches standard configuration extension pattern
	public static IConfigurationBuilder AddYamlFile(
		this IConfigurationBuilder builder,
		string path,
		bool optional,
		bool reloadOnChange)
#pragma warning restore IDE0060
	{
		if (builder == null)
		{
			throw new ArgumentNullException(nameof(builder));
		}

		var fullPath = Path.IsPathRooted(path)
			? path
			: Path.Combine(AppContext.BaseDirectory, path);

		if (!optional && !File.Exists(fullPath))
		{
			throw new FileNotFoundException($"Configuration file '{fullPath}' not found.", fullPath);
		}

		if (File.Exists(fullPath))
		{
			var yamlContent = File.ReadAllText(fullPath);
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();

			var config = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
			var flattened = FlattenDictionary(config);

			builder.AddInMemoryCollection(flattened);
		}

		return builder;
	}

	private static Dictionary<string, string?> FlattenDictionary(
		Dictionary<string, object> source,
		string prefix = "")
	{
		var result = new Dictionary<string, string?>();

		foreach (var kvp in source)
		{
			var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}:{kvp.Key}";

			if (kvp.Value is Dictionary<string, object> nestedDict)
			{
				var nested = FlattenDictionary(nestedDict, key);
				foreach (var nestedKvp in nested)
				{
					result[nestedKvp.Key] = nestedKvp.Value;
				}
			}
			else
			{
				result[key] = kvp.Value?.ToString();
			}
		}

		return result;
	}
}
