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
		/// Print a string to the output stream, followed by a new-line.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void PrintLine(string text);
	}
}
