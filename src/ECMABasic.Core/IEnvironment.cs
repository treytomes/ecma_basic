namespace ECMABasic.Core
{
	/// <summary>
	/// The environment that a program is run in.
	/// </summary>
	public interface IEnvironment : IErrorReporter
	{
		/// <summary>
		/// The full program, ready to execute.
		/// </summary>
		public Program Program { get; }

		/// <summary>
		/// The line number currently being executed.
		/// If the program is not running this value will be null.
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
		/// Clear everything except for the screen.
		/// </summary>
		void Clear();

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

		/// <summary>
		/// Is the given line number defined?
		/// </summary>
		/// <param name="lineNumber">The line number to look for.</param>
		/// <param name="throwsIfMissing">Should a runtime exception be thrown if the line number is not found?</param>
		/// <returns>True or false; does the line number currently exist in the program?</returns>
		bool ValidateLineNumber(int lineNumber, bool throwsIfMissing = false);

		/// <summary>
		/// Push a line number onto the call stack for RETURN-ing from a GOSUB.
		/// </summary>
		/// <param name="lineNumber">The line number to save for later.</param>
		void PushCallStack(int lineNumber);

		/// <summary>
		/// Pop a line number off of the call stack for RETURN-ing from a GOSUB.
		/// </summary>
		/// <returns>The line number to RETURN to.</returns>
		int PopCallStack();

		/// <summary>
		/// Used by the Program evaluator to see if a stop has been requested.
		/// </summary>
		public void CheckForStopRequest();
	}
}
