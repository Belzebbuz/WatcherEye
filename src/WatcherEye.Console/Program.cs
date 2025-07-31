var services = new ServiceCollection();
services.AddWatcherEye();
services.AddLogging(builder =>
{
	builder.ClearProviders();
	builder.AddAnsiConsoleLogger();
	builder.SetMinimumLevel(LogLevel.Trace);
});


var registrar = new CliServiceRegistrar(services);
var app = new CommandApp<DefaultCommand>(registrar);
app.Configure(config =>
{
	config.PropagateExceptions();
});
await app.RunAsync(args);