using System.Collections.Concurrent;
using WatcherEye.Core.CompilationLoader.Abstractions;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Watcher;

internal class SolutionWatcherFactory(ICompilationLoadService loadService, ISearchFilter filter) : ISolutionWatcherFactory
{
	private static readonly ConcurrentDictionary<string, ISolutionWatcher> Watchers = new();
	public async Task<ISolutionWatcher> CreateAsync(string solutionPath, bool forceLoad = false)
	{
		if(forceLoad && Watchers.TryGetValue(solutionPath, out var watcher))
			return watcher;
		
		var compilations =  await loadService.LoadCompilationAsync(solutionPath);
		watcher = new SolutionWatcher(compilations, filter);
		Watchers[solutionPath] = watcher;
		return watcher;
	}
}