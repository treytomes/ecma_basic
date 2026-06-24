namespace ECMABasic55;

internal sealed class CommandLineProps
{
	public string? FilePath { get; init; }
	public string ConfigFile { get; init; } = "appsettings.yaml";
	public bool Debug { get; init; }
}
