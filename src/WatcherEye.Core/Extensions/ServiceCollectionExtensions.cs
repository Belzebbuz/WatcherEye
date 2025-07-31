using Microsoft.Extensions.DependencyInjection;
using WatcherEye.Core.CompilationLoader;
using WatcherEye.Core.CompilationLoader.Abstractions;
using WatcherEye.Core.Documentation;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Collectors;
using WatcherEye.Core.Documentation.EntryPoints;
using WatcherEye.Core.Implementations;
using WatcherEye.Core.Markdown;
using WatcherEye.Core.Markdown.Abstractions;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWatcherEye(this IServiceCollection services)
	{
		services.AddSingleton<IDocumentationService, DocumentationService>();
		services.AddSingleton<ICompilationLoadService, CompilationLoadService>();
		services.AddSingleton<IMarkdownDocumentService, MarkdownDocumentService>();
		services.AddSingleton<ISearchFilter, SearchFilter>();
		services.AddSingleton<IEntryMethodsResolver, EntryMethodsResolver>();
		services.AddSingleton<ISolutionWatcherFactory, SolutionWatcherFactory>();
		services.AddSingleton<IDocumentationCollectorFactory, DocumentationCollectorFactory>();
		return services;
	}
}