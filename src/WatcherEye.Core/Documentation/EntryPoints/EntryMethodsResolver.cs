using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Extensions;
using WatcherEye.Core.Implementations;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;

namespace WatcherEye.Core.Documentation.EntryPoints;

internal class EntryMethodsResolver : IEntryMethodsResolver
{
	public IReadOnlyCollection<ClassDocumentData> GroupMethods(IReadOnlyCollection<ClassDeclarationInfo> infos, in MethodFilterCriteria criteria)
	{
		var result = new List<ClassDocumentData>();
		foreach (var info in infos)
		{
			var methodDeclarations = info.Declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
			var groupedByNameMethods = methodDeclarations.GroupBy(x => x.Identifier.ToString());
			var resultMethods = new List<MethodDocumentData>();
			foreach (var groupedMethod in groupedByNameMethods)
			{
				var resultSyntaxes = new List<MethodDeclarationSyntax>();
				foreach (var methodDeclarationSyntax in groupedMethod)
				{
					if (info.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) is not IMethodSymbol methodSymbol)
						continue;
					if(!ContainsByMethodName(methodSymbol, criteria))
						continue;
					resultSyntaxes.Add(methodDeclarationSyntax);
				}
				
				if (resultSyntaxes.Count == 0)
					continue;
				var methodData = new MethodDocumentData(groupedMethod.Key,
					info.NamedTypeSymbol.ContainingNamespace.ToDisplayString(),
					resultSyntaxes.AsReadOnly());
				resultMethods.Add(methodData);
			}

			if(resultMethods.Count == 0)
				continue;
			var data = new ClassDocumentData(info, resultMethods.AsReadOnly());
			result.Add(data);
		}
		return result;
	}
	
	private static bool ContainsByMethodName(IMethodSymbol? methodSymbol,in MethodFilterCriteria criteria)
	{
		if (methodSymbol is null)
			return false;
		if (criteria.Names.IsNullOrEmpty())
		{
			var isValidAccessibility = methodSymbol.DeclaredAccessibility is Accessibility.Public;
			return isValidAccessibility;
		}
		foreach (var filterMethodName in criteria.Names!)
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
}