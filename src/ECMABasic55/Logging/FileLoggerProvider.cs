using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ECMABasic55.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
	private readonly string _filePath;
	private readonly LogLevel _minLevel;
	private readonly object _lock = new();
	private StreamWriter? _writer;

	public FileLoggerProvider(string filePath, LogLevel minLevel)
	{
		_filePath = filePath;
		_minLevel = minLevel;
		_writer = new StreamWriter(filePath, append: true)
		{
			AutoFlush = true
		};
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new FileLogger(categoryName, this);
	}

	public void Log(string categoryName, LogLevel level, string message)
	{
		if (level < _minLevel)
		{
			return;
		}

		lock (_lock)
		{
			var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
			var levelStr = level.ToString().ToUpperInvariant();
			_writer?.WriteLine($"{timestamp} [{levelStr}] {categoryName}: {message}");
		}
	}

	public void Dispose()
	{
		lock (_lock)
		{
			_writer?.Dispose();
			_writer = null;
		}
	}

	private sealed class FileLogger : ILogger
	{
		private readonly string _categoryName;
		private readonly FileLoggerProvider _provider;

		public FileLogger(string categoryName, FileLoggerProvider provider)
		{
			_categoryName = categoryName;
			_provider = provider;
		}

		public IDisposable? BeginScope<TState>(TState state)
			where TState : notnull => null;

		public bool IsEnabled(LogLevel logLevel) =>
			logLevel >= _provider._minLevel;

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			var message = formatter(state, exception);
			if (exception != null)
			{
				message += Environment.NewLine + exception;
			}

			_provider.Log(_categoryName, logLevel, message);
		}
	}
}
