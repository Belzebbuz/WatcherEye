using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Collectors.Recursive;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Documentation.Collectors;

internal class DocumentationCollectorFactory(ISearchFilter filter) : IDocumentationCollectorFactory
{
	public IDocumentationCollector Create(ISolutionWatcher watcher)
	{
		return new RecursiveDocumentCollector(watcher, filter);
	}
}