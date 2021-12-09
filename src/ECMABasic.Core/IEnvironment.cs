using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	/// <summary>
	/// The environment that a program is run in.
	/// </summary>
	public interface IEnvironment : IErrorReporter
	{
		/// <summary>
		/// The line number currently being executed.
		/// </summary>
		int CurrentLineNumber { get; set; }

		/// <summary>
		/// The terminal row to write to next.
		/// </summary>
		int TerminalRow { get; set; }

		/// <summary>
		/// The terminal column to write to next.
		/// </summary>
		int TerminalColumn { get; set; }

		/// <summary>
		/// Print a string to the output stream, followed by a new-line.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void PrintLine(string text = "");

		/// <summary>
		/// Print a string to the output stream without a new-line.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void Print(string text);

		/// <summary>
		/// Get the value of a variable.
		/// </summary>
		/// <param name="variableName">The variable to retrieve.</param>
		/// <returns>The value of the variable.</returns>
		string GetStringVariableValue(string variableName);

		/// <summary>
		/// Set the value of a variable.
		/// </summary>
		/// <param name="variableName">The name of the value.</param>
		/// <param name="value">The value to assign.</param>
		void SetStringVariableValue(string variableName, string value);

		/// <summary>
		/// Get the value of a variable.
		/// </summary>
		/// <param name="variableName">The variable to retrieve.</param>
		/// <returns>The value of the variable.</returns>
		double GetNumericVariableValue(string variableName);

		/// <summary>
		/// Set the value of a variable.
		/// </summary>
		/// <param name="variableName">The name of the value.</param>
		/// <param name="value">The value to assign.</param>
		void SetNumericVariableValue(string variableName, double value);
	}
}
