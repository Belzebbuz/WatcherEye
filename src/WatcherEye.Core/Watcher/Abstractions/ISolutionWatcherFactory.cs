namespace WatcherEye.Core.Watcher.Abstractions;

public interface ISolutionWatcherFactory
{
	public Task<ISolutionWatcher> CreateAsync(string solutionPath, bool forceLoad = false);
}