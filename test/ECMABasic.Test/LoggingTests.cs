using System;
using ECMABasic.Application;
using ECMABasic.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ECMABasic.Test;

/// <summary>
/// Tests for ILogger integration in BASIC runtime.
/// Verifies that runtime errors are logged to ILogger when available.
/// </summary>
public class LoggingTests
{
	#region Logger Property Tests

	[Fact]
	public void TestEnvironment_WithLogger_ExposesLogger()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();

		// Act
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Assert
		Assert.NotNull(env.Logger);
		Assert.Same(mockLogger.Object, env.Logger);
	}

	[Fact]
	public void TestEnvironment_WithoutLogger_ReturnsNull()
	{
		// Arrange & Act
		var env = new TestEnvironment(logger: null);

		// Assert
		Assert.Null(env.Logger);
	}

	#endregion

	#region ReportError Logging Tests

	[Fact]
	public void ReportError_WithLogger_LogsAtErrorLevel()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);
		env.CurrentLineNumber = 10;

		// Act
		env.ReportError("% TEST ERROR");

		// Assert
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => true),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void ReportError_WithLogger_IncludesLineNumber()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);
		env.CurrentLineNumber = 42;

		// Act
		env.ReportError("% DIVISION BY ZERO");

		// Assert
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("42")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void ReportError_WithLogger_IncludesErrorMessage()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);
		env.CurrentLineNumber = 10;

		// Act
		env.ReportError("% SYNTAX ERROR");

		// Assert
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SYNTAX ERROR")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void ReportError_WithoutLogger_DoesNotThrow()
	{
		// Arrange
		var env = new TestEnvironment(logger: null);

		// Act & Assert (should not throw)
		env.ReportError("% TEST ERROR");
	}

	[Fact]
	public void ReportError_WithoutLineNumber_LogsWithoutLineNumber()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);
		env.CurrentLineNumber = 0; // No line number

		// Act
		env.ReportError("% IMMEDIATE MODE ERROR");

		// Assert - should log without line number
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("IMMEDIATE MODE ERROR")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	#endregion

	#region Integration Tests

	[Fact]
	public void RuntimeError_LogsToILogger()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Act - Run program with LOG(-5) which raises RuntimeException
		Interpreter.FromText("10 PRINT LOG(-5)\n20 END\n", env);
		env.Program.Execute(env);

		// Assert - Error should be logged
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("LOG")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void RuntimeError_LogsWithCorrectLineNumber()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Act - Error occurs on line 25
		Interpreter.FromText("10 PRINT 1\n25 PRINT SQR(-4)\n30 END\n", env);
		env.Program.Execute(env);

		// Assert - Should log with line 25
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("25")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void SuccessfulExecution_DoesNotLog()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Act - Successful program execution
		Interpreter.FromText("10 PRINT 5\n20 END\n", env);
		env.Program.Execute(env);

		// Assert - No errors should be logged
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Never);
	}

	[Fact]
	public void MultipleErrors_LogsEachError()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Act - First error
		Interpreter.FromText("10 PRINT LOG(0)\n20 END\n", env);
		env.Program.Execute(env);

		// Reset for second program
		env.Program.Clear();

		// Act - Second error
		Interpreter.FromText("10 PRINT SQR(-1)\n20 END\n", env);
		env.Program.Execute(env);

		// Assert - Should have logged twice
		mockLogger.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Exactly(2));
	}

	#endregion

	#region Console Output Preservation Tests

	[Fact]
	public void ReportError_WithLogger_StillPrintsToConsole()
	{
		// Arrange
		var mockLogger = new Mock<ILogger>();
		var env = new TestEnvironment(logger: mockLogger.Object);

		// Act
		env.ReportError("% TEST ERROR");

		// Assert - Error should appear in console output
		var output = env.Text;
		Assert.Contains("TEST ERROR", output);
	}

	[Fact]
	public void ReportError_WithoutLogger_StillPrintsToConsole()
	{
		// Arrange
		var env = new TestEnvironment(logger: null);

		// Act
		env.ReportError("% TEST ERROR");

		// Assert - Error should appear in console output
		var output = env.Text;
		Assert.Contains("TEST ERROR", output);
	}

	#endregion
}
