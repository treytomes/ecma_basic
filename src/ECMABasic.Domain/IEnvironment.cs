namespace ECMABasic.Domain;

/// <summary>
/// The environment that a program is run in.
/// Provides I/O operations and runtime context.
/// </summary>
public interface IEnvironment : IErrorReporter
{
	/// <summary>
	/// The configuration settings for this BASIC implementation.
	/// </summary>
	public IBasicConfiguration Configuration { get; }

	/// <summary>
	/// The line number currently being executed.
	/// If the program is not running this value will be null.
	/// </summary>
	public int CurrentLineNumber { get; set; }

	/// <summary>
	/// The terminal row to write to next.
	/// </summary>
	public int TerminalRow { get; set; }

	/// <summary>
	/// The terminal column to write to next.
	/// </summary>
	public int TerminalColumn { get; set; }

	/// <summary>
	/// Clear everything except for the screen.
	/// </summary>
	public void Clear();

	/// <summary>
	/// Read a line from the input stream.
	/// </summary>
	/// <returns>The line that was read.</returns>
	public string ReadLine();

	/// <summary>
	/// Print a string to the output stream, followed by a new-line.
	/// </summary>
	/// <param name="text">The text to print.</param>
	public void PrintLine(string text = "");

	/// <summary>
	/// Print a string to the output stream without a new-line.
	/// </summary>
	/// <param name="text">The text to print.</param>
	public void Print(string text);

	/// <summary>
	/// Get the value of a string variable.
	/// </summary>
	/// <param name="variableName">The variable to retrieve.</param>
	/// <returns>The value of the variable.</returns>
	public string GetStringVariableValue(string variableName);

	/// <summary>
	/// Set the value of a string variable.
	/// </summary>
	/// <param name="variableName">The name of the variable.</param>
	/// <param name="value">The value to assign.</param>
	public void SetStringVariableValue(string variableName, string value);

	/// <summary>
	/// Get the value of a numeric variable.
	/// </summary>
	/// <param name="variableName">The variable to retrieve.</param>
	/// <returns>The value of the variable.</returns>
	public double GetNumericVariableValue(string variableName);

	/// <summary>
	/// Set the value of a numeric variable.
	/// </summary>
	/// <param name="variableName">The name of the variable.</param>
	/// <param name="value">The value to assign.</param>
	public void SetNumericVariableValue(string variableName, double value);

	/// <summary>
	/// Is the given line number defined?
	/// </summary>
	/// <param name="lineNumber">The line number to look for.</param>
	/// <param name="throwsIfMissing">Should a runtime exception be thrown if the line number is not found?</param>
	/// <returns>True or false; does the line number currently exist in the program?</returns>
	public bool ValidateLineNumber(int lineNumber, bool throwsIfMissing = false);

	/// <summary>
	/// Push a context onto the call stack for RETURN-ing from a GOSUB or resetting a loop.
	/// </summary>
	/// <param name="context">The context to use on the next iteration of whatever.</param>
	public void PushCallStack(ICallStackContext context);

	/// <summary>
	/// Pop a context off of the call stack for RETURN-ing from a GOSUB or resetting a loop.
	/// </summary>
	/// <returns>The context we're currently working in.</returns>
	public ICallStackContext? PopCallStack();

	/// <summary>
	/// Used by the Program evaluator to see if a stop has been requested.
	/// </summary>
	public void CheckForStopRequest();

	/// <summary>
	/// Load and interpret a file.
	/// </summary>
	/// <param name="filename">The file to load.</param>
	public bool LoadFile(string filename);

	/// <summary>
	/// Read the next datum and increment the index.
	/// </summary>
	/// <returns>The next piece of data.</returns>
	public IExpression ReadData();

	/// <summary>
	/// Reset the data pointer to the first datum.
	/// </summary>
	public void ResetDataPointer();

	/// <summary>
	/// Get the next line number after the given line number.
	/// </summary>
	/// <param name="fromLineNumber">The line number to start from.</param>
	/// <returns>The next line number, or -1 if no more lines.</returns>
	public int GetNextLineNumber(int fromLineNumber);

	/// <summary>
	/// Move to the next line in the program and return the statement.
	/// Updates CurrentLineNumber.
	/// </summary>
	/// <returns>The statement at the next line, or null if no more lines.</returns>
	public IStatement? MoveToNextLine();

	/// <summary>
	/// Get the statement at the given line number.
	/// </summary>
	/// <param name="lineNumber">The line number to retrieve.</param>
	/// <returns>The statement at that line, or null if line doesn't exist.</returns>
	public IStatement? GetStatementAtLine(int lineNumber);
}
