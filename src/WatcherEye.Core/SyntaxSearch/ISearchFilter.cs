using Microsoft.CodeAnalysis;

namespace WatcherEye.Core.SyntaxSearch;

public interface ISearchFilter
{
	public bool ContainsDependencyMethodByNamespace(IMethodSymbol methodSymbol,in SearchCriteria criteria);
	public bool ContainsDependencyMethodByNamespace(IMethodSymbol methodSymbol,DiagramNamespaceFilterCriteria criteria);
	public bool Contains(ITypeSymbol? typeSymbol,in SearchCriteria criteria);
	public bool ContainsByMethodName(IMethodSymbol? methodSymbol,in SearchCriteria criteria);
}