using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class RecursiveDocumentCollector(ISolutionWatcher watcher, ISearchFilter filter) : IDocumentationCollector
{
	public OverloadDocumentation GetDocumentation(
		MethodDeclarationSyntax methodSyntax,
		ClassDeclarationInfo info,
		MethodDocumentation parentDocumentation, 
		DiagramNamespaceFilterCriteria methodNamespaces)
	{
		var methodSymbol = info.SemanticModel.GetDeclaredSymbol(methodSyntax) ??
		                   throw new ArgumentException(nameof(methodSyntax));
		var context = new OverloadDocumentationContext
		{
			ClassDeclarationInfo = info,
			RootSyntax = methodSyntax,
			RootMethodSymbol = methodSymbol,
			StatementCollector = new StatementCollector(),
			DependencyCollector = new RecursiveDependencyCollector(new DependencyCache()),
			ParentDocumentation = parentDocumentation,
			SearchFilter = filter,
			Watcher = watcher,
			DiagramInvocationNamespaces = methodNamespaces
		};
		var walker = new OverloadDocumentationWalker(context);
		walker.VisitMethodDeclaration(context.RootSyntax);
		return context.BuildDocumentation();
	}
}