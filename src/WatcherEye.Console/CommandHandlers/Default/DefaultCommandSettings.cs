namespace WatcherEye.Console.CommandHandlers.Default;

public class DefaultCommandSettings : CommandSettings
{
	[CommandArgument(0, "[solutionPath]")]
	public string? SolutionPath { get; set; }

	[CommandOption("-c|--class")]
	public string[]? ClassName { get; set; }

	[CommandOption("-m|--method")]
	public string[]? MethodName { get; set; }
	
	[CommandOption("-i|--interface")]
	public string[]? InterfaceName { get; set; }
	
	[CommandOption("-n|--namespaces")]
	public string[]? TypeNamespaces { get; set; }

	[CommandOption("-u| --url")]
	public string? RemoteRepositoryBasePath { get; set; }

	[CommandOption("-o|--output")]
	public string? OutputFolder { get; set; }

	[CommandOption("-d|--diagram-namespaces")]
	public string[]? IncludeDependencyMethodNamespace { get; set; }
}