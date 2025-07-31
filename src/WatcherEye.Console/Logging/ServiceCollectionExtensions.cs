using Microsoft.Extensions.Logging;

namespace WatcherEye.Console.Logging;

public static class ServiceCollectionExtensions
{
	public static ILoggingBuilder AddAnsiConsoleLogger(
		this ILoggingBuilder builder)
	{
		builder.AddProvider(new AnsiLoggerProvider());
		return builder;
	}
}