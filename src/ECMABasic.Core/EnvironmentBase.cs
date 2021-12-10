using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	public abstract class EnvironmentBase : IEnvironment
	{
		private readonly Dictionary<string, string> _stringVariables = new Dictionary<string, string>();
		private readonly Dictionary<string, double> _numericVariables = new Dictionary<string, double>();

		private IBasicConfiguration _config;

		public EnvironmentBase(IBasicConfiguration config = null)
		{
			_config = config ?? MinimalBasicConfiguration.Instance;
		}

		/// <summary>
		/// The line number currently being executed.
		/// </summary>
		public int CurrentLineNumber { get; set; }

		public abstract int TerminalRow { get; set; }

		public abstract int TerminalColumn { get; set; }

		public abstract void Print(string text);

		public abstract void PrintLine(string text = "");

		public abstract void ReportError(string message);

		public string GetStringVariableValue(string variableName)
		{
			// TODO: Validate variable name?
			if (!_stringVariables.ContainsKey(variableName))
			{
				var value = string.Empty;
				SetStringVariableValue(variableName, value);
				return value;
			}
			return _stringVariables[variableName];
		}
		
		public void SetStringVariableValue(string variableName, string value)
		{
			// TODO: Validate variable name?
			if (value.Length > _config.MaxStringLength)
			{
				throw new LineRuntimeException("STRING OVERFLOW", CurrentLineNumber);
			}
			_stringVariables[variableName] = value;
		}

		public double GetNumericVariableValue(string variableName)
		{
			// TODO: Validate variable name?
			if (!_numericVariables.ContainsKey(variableName))
			{
				var value = 0;
				SetNumericVariableValue(variableName, value);
				return value;
			}
			return _numericVariables[variableName];
		}

		public void SetNumericVariableValue(string variableName, double value)
		{
			// TODO: Validate variable name?
			_numericVariables[variableName] = value;
		}
	}
}
