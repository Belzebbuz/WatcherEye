using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Documentation.Abstractions;

internal interface IDocumentationCollectorFactory
{
	public IDocumentationCollector Create(ISolutionWatcher watcher);
}