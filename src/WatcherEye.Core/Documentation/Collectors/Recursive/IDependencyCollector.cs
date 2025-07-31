using Microsoft.CodeAnalysis;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public interface IDependencyCollector
{
	IReadOnlyCollection<IDependency> Dependencies { get; }

	IReadOnlyCollection<OverloadDocumentation> Collect(OverloadDocumentationContext context,
		IMethodSymbol externalMethodSymbol, SyntaxNode? targetFakeSyntax);
}