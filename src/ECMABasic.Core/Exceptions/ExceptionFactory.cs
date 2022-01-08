using ECMABasic.Core.Exceptions;
using System;

namespace ECMABasic.Core
{
	public static class ExceptionFactory
	{
		public static Exception Syntax()
		{
			return new SyntaxException("SYNTAX ERROR");
		}

		public static Exception ProgramStop(int? lineNumber)
		{
			return new ProgramStopException(lineNumber);
		}

		public static Exception ProgramEnd(int? lineNumber)
		{
			return new ProgramEndException(lineNumber);
		}

		public static Exception NoEndInstruction()
		{
			return new NoEndInstructionException();
		}

		public static Exception EndIsNotLast(int? lineNumber)
		{
			return new SyntaxException("END IS NOT LAST", lineNumber);
		}

		public static Exception LineNumberExpected()
		{
			return new SyntaxException("EXPECTED A LINE NUMBER");
		}

		public static Exception LineTooLong(int tooLongBy, int? lineNumber = null)
		{
			return new SyntaxException($"LINE IS TOO LONG BY {tooLongBy} CHARACTERS", lineNumber);
		}

		public static Exception UndefinedFunction(int? lineNumber = null)
		{
			return new SyntaxException("UNDEFINED FUNCTION", lineNumber);
		}

		public static Exception NotAllowedInProgram(int? lineNumber = null)
		{
			return new RuntimeException("NOT ALLOWED IN PROGRAM", lineNumber);
		}

		public static Exception OnlyAllowedInProgram(int? lineNumber = null)
		{
			return new RuntimeException("ONLY ALLOWED BE IN PROGRAM", lineNumber);
		}

		public static Exception UndefinedLineNumber(int testLineNumber, int? lineNumber = null)
		{
			return new RuntimeException($"UNDEFINED LINE NUMBER {testLineNumber}", lineNumber);
		}

		public static Exception LineNumberOutOfRange(int testLineNumber, int? lineNumber = null)
		{
			return new RuntimeException($"LINE NUMBER {testLineNumber} OUT OF RANGE", lineNumber);
		}

		public static Exception ForWithoutNext(int? lineNumber = null)
		{
			return new SyntaxException("FOR WITHOUT NEXT", lineNumber);
		}

		public static Exception NextWithoutFor(int? lineNumber = null)
		{
			return new SyntaxException("NEXT WITHOUT FOR", lineNumber);
		}

		public static Exception ForUsingPreviousControlVariable(int? lineNumber = null)
		{
			return new SyntaxException("FOR USING PREVIOUS CONTROL-VARIABLE", lineNumber);
		}

		public static Exception ControlTransferIntoForBlock(int? lineNumber = null)
		{
			return new RuntimeException("CONTROL TRANSFER INTO FOR-BLOCK", lineNumber);
		}

		public static Exception ReturnWithoutGosub(int? lineNumber = null)
		{
			return new RuntimeException("RETURN WITHOUT GOSUB", lineNumber);
		}

		public static Exception MixedStringsAndNumbers(int? lineNumber = null)
		{
			return new SyntaxException("MIXED STRINGS AND NUMBERS", lineNumber);
		}

		public static Exception OutOfData(int? lineNumber = null)
		{
			return new RuntimeException("OUT OF DATA", lineNumber);
		}

		public static Exception ExpectedConditionalExpression(int? lineNumber = null)
		{
			return new SyntaxException("EXPECTED A CONDITIONAL EXPRESSION", lineNumber);
		}

		public static Exception ExpectedStringExpression(int? lineNumber = null)
		{
			return new SyntaxException("EXPECTED A STRING EXPRESSION", lineNumber);
		}

		public static Exception ExpectedNumericExpression(int? lineNumber = null)
		{
			return new SyntaxException("EXPECTED A NUMERIC EXPRESSION", lineNumber);
		}

		public static Exception ExpectedNumericVariable(int? lineNumber = null)
		{
			return new SyntaxException("EXPECTED A NUMERIC VARIABLE", lineNumber);
		}

		public static Exception ExpectedVariable(int? lineNumber = null)
		{
			return new SyntaxException("EXPECTED A VARIABLE", lineNumber);
		}

		public static Exception ExpectedData(int? lineNumber = null)
		{
			return new RuntimeException("EXPECTED A DATA STATEMENT", lineNumber);
		}

		public static Exception StringOverflow(int? lineNumber = null)
		{
			return new RuntimeException("STRING OVERFLOW", lineNumber);
		}

		public static Exception ArgumentCountMismatch(int? lineNumber = null)
		{
			return new SyntaxException("ARGUMENT COUNT MISMATCH", lineNumber);
		}

		public static Exception ArgumentTypeMismatch(int? lineNumber = null)
		{
			return new SyntaxException("ARGUMENT TYPE MISMATCH", lineNumber);
		}

		public static Exception IllegalOperator(int? lineNumber = null)
		{
			return new SyntaxException("ILLEGAL OPERATOR", lineNumber);
		}

		public static Exception IllegalFormula(int? lineNumber = null)
		{
			return new SyntaxException("ILLEGAL FORMULA", lineNumber);
		}

		public static Exception IndexOutOfRange(int? lineNumber = null)
		{
			return new RuntimeException("INDEX OUT OF RANGE", lineNumber);
		}
	}
}
