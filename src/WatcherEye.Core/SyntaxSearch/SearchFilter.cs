using Microsoft.CodeAnalysis;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.SyntaxSearch;

public class SearchFilter : ISearchFilter
{
	public bool ContainsDependencyMethodByNamespace(IMethodSymbol methodSymbol,in SearchCriteria criteria)
	{
		var dependencyNamespace = methodSymbol.ContainingNamespace.ToDisplayString();
		if(criteria.IncludeDependencyMethodNamespace.IsNullOrEmpty())
			return true;
		
		foreach (var methodNamespace in criteria.IncludeDependencyMethodNamespace!)
		{
			var isContainsFilter = methodNamespace.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when dependencyNamespace
					.StartsWith(methodNamespace[..^1], StringComparison.InvariantCultureIgnoreCase):
				case false when dependencyNamespace
					.Equals(methodNamespace, StringComparison.InvariantCultureIgnoreCase):
					return true;
			}
		}

		return false;
	}

	public bool ContainsDependencyMethodByNamespace(IMethodSymbol methodSymbol, DiagramNamespaceFilterCriteria criteria)
	{
		var dependencyNamespace = methodSymbol.ContainingNamespace.ToDisplayString();
		if(criteria.Namespaces.IsNullOrEmpty())
			return true;
		
		foreach (var methodNamespace in criteria.Namespaces!)
		{
			var isContainsFilter = methodNamespace.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when dependencyNamespace
					.StartsWith(methodNamespace[..^1], StringComparison.InvariantCultureIgnoreCase):
				case false when dependencyNamespace
					.Equals(methodNamespace, StringComparison.InvariantCultureIgnoreCase):
					return true;
			}
		}

		return false;
	}

	public bool Contains(ITypeSymbol? typeSymbol,in SearchCriteria criteria)
	{
		if (typeSymbol is null)
			return false;
		return ContainsByNamespace(typeSymbol, in criteria) && ContainsByClassName(typeSymbol, in criteria) && ContainsByInterfaceName(typeSymbol, in criteria); 
	}

	public bool ContainsByMethodName(IMethodSymbol? methodSymbol,in SearchCriteria criteria)
	{
		if (methodSymbol is null)
			return false;
		if (criteria.MethodName.IsNullOrEmpty())
		{
			var isValidAccessibility = methodSymbol.DeclaredAccessibility is Accessibility.Public;
			return isValidAccessibility;
		}
		foreach (var filterMethodName in criteria.MethodName!)
		{
			var isContainsFilter = filterMethodName.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when methodSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
					.StartsWith(filterMethodName[..^1], StringComparison.InvariantCultureIgnoreCase):
				case false when methodSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
					.Equals(filterMethodName, StringComparison.InvariantCultureIgnoreCase):
					return true;
			}
		}
		return false;
	}
	
	private bool ContainsByClassName(ITypeSymbol classSymbol,in SearchCriteria criteria)
	{
		if(criteria.ClassNames.IsNullOrEmpty())
			return true;
		
		foreach (var filterClassName in criteria.ClassNames!)
		{
			var isContainsFilter = filterClassName.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).StartsWith(filterClassName[..^1],
					StringComparison.InvariantCultureIgnoreCase):
				case false when classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Equals(filterClassName, StringComparison.InvariantCultureIgnoreCase):
				{
					return true;
				} 
			}
		}
		return false;
	}

	private bool ContainsByInterfaceName(ITypeSymbol classSymbol, in SearchCriteria criteria)
	{
		if(criteria.InterfaceNames.IsNullOrEmpty())
			return true;
		
		foreach (var interfaceName in criteria.InterfaceNames!)
		{
			var isContainsFilter = interfaceName.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when classSymbol.AllInterfaces.Any(x =>
					x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).StartsWith(interfaceName[..^1], StringComparison.InvariantCultureIgnoreCase)):
				case false when classSymbol.AllInterfaces.Any(x =>
					x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Equals(interfaceName, StringComparison.InvariantCultureIgnoreCase)):
				{
					return true;
				}
			}
		}
		return false;
	}
	
	private bool ContainsByNamespace(ITypeSymbol classSymbol, in SearchCriteria criteria)
	{
		var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
		if(string.IsNullOrEmpty(namespaceName))
			return true;
		if(criteria.TypeNamespaces.IsNullOrEmpty())
			return true;
		foreach (var filterNamespace in criteria.TypeNamespaces!)
		{
			var isContainsFilter = filterNamespace.EndsWith("*");
			switch (isContainsFilter)
			{
				case true when namespaceName.StartsWith(filterNamespace[..^1], StringComparison.InvariantCultureIgnoreCase):
				case false when namespaceName.Equals(filterNamespace, StringComparison.InvariantCultureIgnoreCase):
					return true;
			}
		}
		return false;
	}
}