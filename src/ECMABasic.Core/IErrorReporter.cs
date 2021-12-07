using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	/// <summary>
	/// Handles storing up errors from either interpreting, compiling, or runtime.
	/// </summary>
	public interface IErrorReporter
	{
		/// <summary>
		/// Record an error for later viewing.
		/// </summary>
		/// <param name="message">The error message to record.</param>
		void ReportError(string message);
	}
}
