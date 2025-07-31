using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Comparers;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.Watcher;

public record ClassDeclarationInfo(ClassDeclarationSyntax Declaration, SemanticModel SemanticModel)
{
	private const string InheritComment = "inheritdoc";
	public INamedTypeSymbol NamedTypeSymbol { get; } =
		SemanticModel.GetDeclaredSymbol(Declaration) as INamedTypeSymbol ?? throw new ArgumentException();

	public string FullName => NamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
	public string Name => NamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
	public string? XmlComment => NamedTypeSymbol.GetDocumentationCommentXml();
	public string GetPath() => Declaration.GetPath();
	public IMethodSymbol? GetMethod(MethodDeclarationSyntax methodSyntax)
	{
		return SemanticModel.GetDeclaredSymbol(methodSyntax) as IMethodSymbol;
	}


	public MethodDeclarationSyntax? FindImplementationSyntax(IMethodSymbol methodSymbol)
	{
		var originalInterfaceMethod = NamedTypeSymbol.AllInterfaces
			.FirstOrDefault(iFace => SymbolByNameEqualityComparer.Instance.Equals(iFace.OriginalDefinition, methodSymbol.ContainingType.OriginalDefinition))?
			.GetMembers().OfType<IMethodSymbol>()
			.FirstOrDefault(i => SymbolByNameEqualityComparer.Instance.Equals(i.OriginalDefinition, methodSymbol.OriginalDefinition));
		if (originalInterfaceMethod is null)
			return null;
		var implementation = NamedTypeSymbol.FindImplementationForInterfaceMember(originalInterfaceMethod);
		return implementation?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
	}
	
	public IMethodSymbol? FindImplementationSymbol(IMethodSymbol methodSymbol)
	{
		var originalInterfaceMethod = NamedTypeSymbol.AllInterfaces
			.FirstOrDefault(iFace => SymbolByNameEqualityComparer.Instance.Equals(iFace.OriginalDefinition, methodSymbol.ContainingType.OriginalDefinition))?
			.GetMembers().OfType<IMethodSymbol>()
			.FirstOrDefault(i => SymbolByNameEqualityComparer.Instance.Equals(i.OriginalDefinition, methodSymbol.OriginalDefinition));
		if (originalInterfaceMethod is null)
			return null;
		var implementation = NamedTypeSymbol.FindImplementationForInterfaceMember(originalInterfaceMethod);
		return implementation as IMethodSymbol;
	}
	
	public string? FindFirstImplementationXmlComment(IMethodSymbol symbol)
	{
		var sourceComment = symbol.GetDocumentationCommentXml();
		if(!sourceComment.IsNullOrEmpty() && !sourceComment!.Contains(InheritComment))
			return sourceComment;
		
		var implementationComment = NamedTypeSymbol.AllInterfaces
			.SelectMany(x => x.GetMembers().OfType<IMethodSymbol>())
			.FirstOrDefault(x => MethodSymbolImplementationComparer.Instance.Equals(x, symbol)
			                     && !string.IsNullOrEmpty(x.GetDocumentationCommentXml())
			                     && !(x.GetDocumentationCommentXml()?.Contains(InheritComment) ?? false));
		return implementationComment?.GetDocumentationCommentXml();
	}
}

public record InterfaceDeclarationInfo(
	InterfaceDeclarationSyntax Declaration,
	SemanticModel SemanticModel,
	IReadOnlyCollection<ClassDeclarationInfo> Implementations)
{
	public INamedTypeSymbol NamedTypeSymbol { get; } =
		SemanticModel.GetDeclaredSymbol(Declaration) as INamedTypeSymbol ?? throw new ArgumentException();

	public string GetPath() => Declaration.GetPath();

	public IMethodSymbol? GetMethod(MethodDeclarationSyntax methodSyntax)
	{
		var originalMethodSymbol = Declaration.DescendantNodes()
			.OfType<MethodDeclarationSyntax>()
			.FirstOrDefault(x => SyntaxNodeComparer.Instance.Equals(x, methodSyntax));
		if (originalMethodSymbol is null)
			return null;
		return SemanticModel.GetDeclaredSymbol(originalMethodSymbol) as IMethodSymbol;
	}
}