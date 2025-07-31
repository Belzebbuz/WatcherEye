using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Extensions;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class OverloadDocumentationContext
{
	public required ClassDeclarationInfo ClassDeclarationInfo { get; init; }
	public SemanticModel SemanticModel => ClassDeclarationInfo.SemanticModel;
	public required MethodDeclarationSyntax RootSyntax { get; init; }
	public required IMethodSymbol RootMethodSymbol { get; init; }
	public required IStatementCollector StatementCollector { get; init; }
	public required ISearchFilter SearchFilter { get; init; }
	public required IDependencyCollector DependencyCollector { get; init; }
	public required ISolutionWatcher Watcher { get; init; }
	public required MethodDocumentation ParentDocumentation { get; init; }
	public required DiagramNamespaceFilterCriteria DiagramInvocationNamespaces { get; init; }

	public OverloadDocumentation BuildDocumentation()
	{
		var returnType = new ReturnType
		{
			Name = RootMethodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
			TypeArguments = (RootMethodSymbol.ReturnType as INamedTypeSymbol)?.GetTypeArguments() ?? []
		};
		var comment = ClassDeclarationInfo.FindFirstImplementationXmlComment(RootMethodSymbol);
		return new OverloadDocumentation(StatementCollector.Statements.ToList())
		{
			Parent = ParentDocumentation,
			OverloadName = RootMethodSymbol.GetOverloadName(),
			FullSignature = RootMethodSymbol.GetFullSignatureName(),
			ReturnType = returnType,
			DeclarationUrl = RootSyntax.GetPath(),
			Parameters = RootMethodSymbol.GetParameters(),
			TypeParameters = RootMethodSymbol.GetTypeParameters(),
			Dependencies = DependencyCollector.Dependencies,
			XmlComment = comment
		};
	}
}