using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Dependencies;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Watcher;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class RecursiveDependencyCollector(IDependencyCache cache) : IDependencyCollector
{
	public IReadOnlyCollection<IDependency> Dependencies => cache.Dependencies;

	public IReadOnlyCollection<OverloadDocumentation> Collect(
		OverloadDocumentationContext context,
		IMethodSymbol externalMethodSymbol,
		SyntaxNode? targetFakeSyntax)
	{
		switch (targetFakeSyntax)
		{
			case ClassDeclarationSyntax classSyntax:
				return CollectImplementationDependencies(context, classSyntax, externalMethodSymbol);
			case InterfaceDeclarationSyntax interfaceSyntax:
				return CollectInterfaceDependencies(context, interfaceSyntax, externalMethodSymbol);
			default:
				return [];
		}
	}
	private record CreateDependencyResult(ClassDependency ClassDependency, OverloadDocumentation OverloadDocumentation);
	private IReadOnlyCollection<OverloadDocumentation> CollectInterfaceDependencies(
		OverloadDocumentationContext context,
		InterfaceDeclarationSyntax interfaceSyntax,
		IMethodSymbol externalMethodSymbol)
	{
		if (externalMethodSymbol.IsExtensionMethod)
			return [];
		var originalInterfaceInfo = context.Watcher.FindDeclarationsInfo(interfaceSyntax);
		var fakeMethodSyntax = externalMethodSymbol.DeclaringSyntaxReferences
			.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

		if (originalInterfaceInfo is null || fakeMethodSyntax is null)
			return [];

		var originalInterfaceMethodSymbol = originalInterfaceInfo.GetMethod(fakeMethodSyntax);
		var originalMethodSyntax =
			originalInterfaceMethodSymbol?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as
				MethodDeclarationSyntax;
		if (originalInterfaceMethodSymbol is null || originalMethodSyntax is null)
			return [];

		var implementationDocuments = originalInterfaceInfo.Implementations.Select(info =>
		{
			var implementationMethodSymbol = info.FindImplementationSymbol(originalInterfaceMethodSymbol);
			if (implementationMethodSymbol is null)
				return null;
			var dependency = GetDependency(info);
			var documentation = GetDocumentation(context, dependency, implementationMethodSymbol, info);
			if (documentation is null)
				return null;
			dependency.TryAddDocumentation(implementationMethodSymbol, documentation);
			return new CreateDependencyResult(dependency, documentation);
		}).OfType<CreateDependencyResult>().ToArray();
		
		cache.AddOrUpdate(originalInterfaceInfo.NamedTypeSymbol, _ => new InterfaceDependency
		{
			TypeName = originalInterfaceInfo.Declaration.Identifier.ToString(),
			TypeDeclarationUrl = originalInterfaceInfo.GetPath(),
			Namespace = originalInterfaceInfo.NamedTypeSymbol.ContainingNamespace.ToDisplayString(),
			Implementations = [..implementationDocuments.Select(x => x.ClassDependency)],
			XmlDescription = originalInterfaceInfo.NamedTypeSymbol.GetDocumentationCommentXml()
		}, (_, dependency) =>
		{
			((InterfaceDependency)dependency).AddDependencies(
				[..implementationDocuments.Select(x => x.ClassDependency)]);
			return dependency;
		});
		return implementationDocuments.Select(x => x.OverloadDocumentation).ToArray().AsReadOnly();
	}

	private IReadOnlyCollection<OverloadDocumentation> CollectImplementationDependencies(
		OverloadDocumentationContext context,
		ClassDeclarationSyntax externalClassSyntax,
		IMethodSymbol externalMethodSymbol)
	{
		if (externalMethodSymbol.IsExtensionMethod)
			return [];

		var originalClassInfo = context.Watcher.FindDeclarationsInfo(externalClassSyntax);
		var fakeMethodSyntax = externalMethodSymbol.DeclaringSyntaxReferences
			.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

		if (fakeMethodSyntax is null || originalClassInfo is null)
			return [];

		var originalMethodSymbol = originalClassInfo.GetMethod(fakeMethodSyntax);
		
		if (originalMethodSymbol is null)
			return [];

		var dependency = GetDependency(originalClassInfo);
		var documentation = GetDocumentation(context, dependency, originalMethodSymbol, originalClassInfo);
		if (documentation is null)
			return [];
		cache.AddOrUpdate(originalClassInfo.NamedTypeSymbol, _ =>
		{
			dependency.TryAddDocumentation(originalMethodSymbol, documentation);
			return dependency;
		}, (_, classDependency) =>
		{
			((ClassDependency)classDependency).TryAddDocumentation(originalMethodSymbol, documentation);
			return classDependency;
		});
		return [documentation];
	}

	private OverloadDocumentation? GetDocumentation(
		OverloadDocumentationContext context, 
		ClassDependency dependency,
		IMethodSymbol originalMethodSymbol, 
		ClassDeclarationInfo originalClassInfo)
	{
		var originalMethodSyntax =
			originalMethodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
		if(originalMethodSyntax is null)
			return null;
		
		var existsDocumentation = dependency.MethodExists(originalMethodSymbol);
		if (existsDocumentation is not null)
			return existsDocumentation;

		var documentation = cache.GetOrAdd($"{originalMethodSymbol.ContainingType.ToDisplayString()}.{originalMethodSyntax.Identifier.ToString()}", new MethodDocumentation
			{
				MethodName = originalMethodSyntax.Identifier.ToString(),
				ParentTypeName = originalClassInfo.Name,
				Namespace = originalMethodSymbol.ContainingNamespace.ToDisplayString(),
			});
		var newContext = new OverloadDocumentationContext
		{
			ClassDeclarationInfo = originalClassInfo,
			RootSyntax = originalMethodSyntax,
			RootMethodSymbol = originalMethodSymbol,
			StatementCollector = new StatementCollector(),
			SearchFilter = context.SearchFilter,
			DependencyCollector = this,
			ParentDocumentation = documentation,
			DiagramInvocationNamespaces = context.DiagramInvocationNamespaces,
			Watcher = context.Watcher,
		};
		new OverloadDocumentationWalker(newContext).VisitMethodDeclaration(newContext.RootSyntax);
		var overloadDocumentation = newContext.BuildDocumentation();
		documentation.AddOverload(overloadDocumentation);
		return overloadDocumentation;
	}

	private ClassDependency GetDependency(ClassDeclarationInfo originalClassInfo)
	{
		var dependency = cache.GetOrAdd(originalClassInfo.NamedTypeSymbol, new ClassDependency
		{
			TypeName = originalClassInfo.Declaration.Identifier.ToString(),
			TypeDeclarationUrl = originalClassInfo.GetPath(),
			Namespace = originalClassInfo.NamedTypeSymbol.ContainingNamespace.ToDisplayString(),
			XmlDescription = originalClassInfo.NamedTypeSymbol.GetDocumentationCommentXml(),
		});
		return dependency;
	}
}