using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace WatcherEye.Console.Logging;

public class AnsiConsoleLogger : ILogger
{
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel)) return;
        
		var message = formatter(state, exception);
		if (string.IsNullOrEmpty(message)) return;

		switch (logLevel)
		{
			case LogLevel.Trace:
				AnsiConsole.MarkupLine($"[grey]TRACE:[/] {message}");
				break;
			case LogLevel.Debug:
				AnsiConsole.MarkupLine($"[blue]DEBUG:[/] {message}");
				break;
			case LogLevel.Information:
				AnsiConsole.MarkupLine($"[green]INFO:[/] {message}");
				break;
			case LogLevel.Warning:
				AnsiConsole.MarkupLine($"[yellow]WARN:[/] {message}");
				break;
			case LogLevel.Error:
				AnsiConsole.MarkupLine($"[red]ERROR:[/] {message}");
				break;
			case LogLevel.Critical:
				AnsiConsole.MarkupLine($"[bold red]CRITICAL:[/] {message}");
				break;
			default:
				AnsiConsole.WriteLine(message);
				break;
		}

		if (exception != null)
		{
			AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
		}
	}

	public bool IsEnabled(LogLevel logLevel) => true;

	public IDisposable? BeginScope<TState>(TState state) 
		where TState : notnull 
		=> NullScope.Instance;

	private class NullScope : IDisposable
	{
		public static NullScope Instance { get; } = new();
		public void Dispose() { }
	}
}