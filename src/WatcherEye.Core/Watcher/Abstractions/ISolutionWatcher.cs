using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.SyntaxSearch;

namespace WatcherEye.Core.Watcher.Abstractions;

public interface ISolutionWatcher
{
	IReadOnlyCollection<ClassDeclarationInfo> FindEntryPoints(SearchCriteria searchCriteria);
	InterfaceDeclarationInfo? FindDeclarationsInfo(InterfaceDeclarationSyntax syntax);
	ClassDeclarationInfo? FindDeclarationsInfo(ClassDeclarationSyntax syntax);
}