using Microsoft.Extensions.Logging;

namespace WatcherEye.Console.Logging;

public class AnsiLoggerProvider : ILoggerProvider
{
	public void Dispose()
	{
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new AnsiConsoleLogger();
	}
}