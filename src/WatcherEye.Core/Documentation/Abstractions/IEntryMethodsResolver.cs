using WatcherEye.Core.Documentation.EntryPoints;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;

namespace WatcherEye.Core.Documentation.Abstractions;

/// <summary>
/// Фильтрует по названиям методы и группирует перегрузки
/// </summary>
internal interface IEntryMethodsResolver
{
	public IReadOnlyCollection<ClassDocumentData> GroupMethods(IReadOnlyCollection<ClassDeclarationInfo> infos, in MethodFilterCriteria criteria);
}