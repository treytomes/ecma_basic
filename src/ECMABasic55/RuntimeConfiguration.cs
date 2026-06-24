
namespace ECMABasic55;

public class RuntimeConfiguration
{
	private const string DEFAULT_PREAMBLE = @"ECMA-55 MINIMAL BASIC RUNTIME ENVIRONMENT
USAGE: ECMABASIC55 {OPTIONAL FILE PATH}";

	public RuntimeConfiguration()
	{
		Preamble = DEFAULT_PREAMBLE;
	}

	public string Preamble { get; init; }

}
