using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Comparers;
using WatcherEye.Core.Extensions;
using WatcherEye.Core.Implementations;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Watcher;

internal class SolutionWatcher(IReadOnlyList<Compilation> compilations, ISearchFilter filter) : ISolutionWatcher
{
	public IReadOnlyCollection<ClassDeclarationInfo> FindEntryPoints(SearchCriteria searchCriteria)
	{
		var entryPoints = new List<ClassDeclarationInfo>();
		foreach (var compilation in compilations)
		{
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var root = syntaxTree.GetRoot();
				var collector = new EntryPointsWalker(semanticModel, searchCriteria, filter);
				collector.Visit(root);
				entryPoints.AddRange(collector.Implementations);
			}
		}

		return entryPoints;
	}

	public InterfaceDeclarationInfo? FindDeclarationsInfo(InterfaceDeclarationSyntax syntax)
	{
		var info = WatcherSyntaxNodeCache<InterfaceDeclarationInfo?>.GetOrCreate(syntax, node => Factory((InterfaceDeclarationSyntax)node));
		
		InterfaceDeclarationInfo? Factory(InterfaceDeclarationSyntax interfaceSyntax)
		{
			var compilation = compilations.FirstOrDefault(x => x.ContainsSyntaxTree(interfaceSyntax.SyntaxTree));
			if (compilation == null)
				return null;
			
			var syntaxTree = compilation.SyntaxTrees.FirstOrDefault(x => SyntaxTreeComparer.Instance.Equals(x, interfaceSyntax.SyntaxTree));
			if(syntaxTree is null)
				return null;
		
			var semanticModel = compilation.GetSemanticModel(syntaxTree);
			var root = syntaxTree.GetRoot();
			if (root.DescendantNodes().FirstOrDefault(x => SyntaxNodeComparer.Instance.Equals(x, interfaceSyntax)) is not InterfaceDeclarationSyntax targetSyntax)
				return null;
		
			var symbol = semanticModel.GetDeclaredSymbol(targetSyntax);
			if (symbol == null)
				return null;

			var implementations = FindImplementations(symbol);
		
			return new InterfaceDeclarationInfo(targetSyntax, semanticModel, implementations);
		}
		
		return info;
	}

	private IReadOnlyCollection<ClassDeclarationInfo> FindImplementations(ISymbol symbol)
	{
		var implementations =
			WatcherSymbolCache<IReadOnlyCollection<ClassDeclarationInfo>>.GetOrCreate(symbol, Factory);

		IReadOnlyCollection<ClassDeclarationInfo> Factory(ISymbol targetSymbol)
		{
			if (targetSymbol is not INamedTypeSymbol typedSymbol)
				return [];
			var result = new List<ClassDeclarationInfo>();
			foreach (var compilation in compilations)
			{
				foreach (var syntaxTree in compilation.SyntaxTrees)
				{
					if(syntaxTree.IsGenerated())
						continue;
					var semanticModel = compilation.GetSemanticModel(syntaxTree);
					var root = syntaxTree.GetRoot();
					var classImplementations = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
						.Where(x =>
						{
							if (semanticModel.GetDeclaredSymbol(x) is not INamedTypeSymbol classSymbol)
								return false;
						
							return classSymbol.AllInterfaces.Any(implSymbol => SymbolByNameEqualityComparer.Instance.Equals(implSymbol.OriginalDefinition, typedSymbol));
						
						})
						.Select(x => new ClassDeclarationInfo(x, semanticModel));
				
					result.AddRange(classImplementations);
				}
			}
			return result;
		}
		return implementations;
	}

	public ClassDeclarationInfo? FindDeclarationsInfo(ClassDeclarationSyntax syntax)
	{
		
		var declarationInfo = WatcherSyntaxNodeCache<ClassDeclarationInfo?>.GetOrCreate(syntax, node => Factory((ClassDeclarationSyntax)node));

		ClassDeclarationInfo? Factory(ClassDeclarationSyntax syntaxNode)
		{
			var compilation = compilations.FirstOrDefault(x => x.ContainsSyntaxTree(syntax.SyntaxTree));
			if (compilation == null)
				return null;
			
			var syntaxTree = compilation.SyntaxTrees.FirstOrDefault(x => SyntaxTreeComparer.Instance.Equals(x, syntaxNode.SyntaxTree));
			if(syntaxTree is null || syntaxTree.IsGenerated())
				return null;
		
			var semanticModel = compilation.GetSemanticModel(syntaxTree);
			var root = syntaxTree.GetRoot();
			if (root.DescendantNodes().FirstOrDefault(x => SyntaxNodeComparer.Instance.Equals(x, syntaxNode)) is not ClassDeclarationSyntax classSyntax)
				return null;

			return new ClassDeclarationInfo(classSyntax, semanticModel);
		}
		return declarationInfo;
	}
}