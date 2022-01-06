using ECMABasic.Core.Configuration;
using ECMABasic.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ECMABasic.Core
{
	public abstract class EnvironmentBase : IEnvironment
	{
		private readonly Dictionary<string, string> _stringVariables = new();
		private readonly Dictionary<string, double> _numericVariables = new();
		private readonly Stack<ICallStackContext> _callStack = new();
		private readonly DataPointer _dataPointer = new();

		private readonly IBasicConfiguration _config;

		public EnvironmentBase(Interpreter interpreter = null, IBasicConfiguration config = null)
		{
			Interpreter = interpreter ?? new Interpreter();
			_config = config ?? MinimalBasicConfiguration.Instance;
			Program = new Program();
		}

		public Interpreter Interpreter { get; }

		/// <summary>
		/// The line number currently being executed.
		/// </summary>
		public int CurrentLineNumber { get; set; }

		public abstract int TerminalRow { get; set; }

		public abstract int TerminalColumn { get; set; }

		public Program Program { get; private set; }

		public abstract string ReadLine();

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
				throw ExceptionFactory.StringOverflow(CurrentLineNumber);
			}
			if (!variableName.EndsWith("$"))
			{
				throw ExceptionFactory.MixedStringsAndNumbers(CurrentLineNumber);
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
			if (variableName.EndsWith("$"))
			{
				throw ExceptionFactory.MixedStringsAndNumbers(CurrentLineNumber);
			}
			_numericVariables[variableName] = value;
		}

		public void Clear()
		{
			Program.Clear();
			_numericVariables.Clear();
			_stringVariables.Clear();
			CurrentLineNumber = 0;
		}

		public bool ValidateLineNumber(int lineNumber, bool throwsIfMissing = false)
		{
			if (!Program.Any(x => x.LineNumber == lineNumber))
			{
				if (throwsIfMissing)
				{
					throw ExceptionFactory.UndefinedLineNumber(lineNumber, CurrentLineNumber);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}

		public void PushCallStack(ICallStackContext context)
		{
			_callStack.Push(context);
		}

		public ICallStackContext PopCallStack()
		{
			if (_callStack.Count == 0)
			{
				return null;
			}
			return _callStack.Pop();
		}

		public abstract void CheckForStopRequest();

		public bool LoadFile(string filename)
		{
			Program.Clear();
			return Interpreter.InterpretProgramFromFile(this, filename);
		}

		public IExpression ReadData()
		{
			var data = Program.GetDataLine(_dataPointer.LineIndex);
			if (data == null)
			{
				throw ExceptionFactory.OutOfData(CurrentLineNumber);
			}
			var datum = data.Datums[_dataPointer.DatumIndex];
			_dataPointer.DatumIndex++;
			if (_dataPointer.DatumIndex >= data.Datums.Count)
			{
				_dataPointer.DatumIndex = 0;
				_dataPointer.LineIndex++;
			}
			return datum;
		}

		public void ResetDataPointer()
		{
			_dataPointer.LineIndex = 0;
			_dataPointer.DatumIndex = 0;
		}
	}
}
