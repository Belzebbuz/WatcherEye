using WatcherEye.Core.Watcher;

namespace WatcherEye.Core.Documentation.EntryPoints;

/// <summary>
/// Мета-информация о методе и его перегрузках
/// </summary>
public record ClassDocumentData(ClassDeclarationInfo Info, IReadOnlyCollection<MethodDocumentData> Methods);