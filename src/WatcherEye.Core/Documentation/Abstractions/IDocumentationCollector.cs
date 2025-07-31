using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher;

namespace WatcherEye.Core.Documentation.Abstractions;

public interface IDocumentationCollector
{
	public OverloadDocumentation GetDocumentation(MethodDeclarationSyntax methodSyntax, ClassDeclarationInfo info, MethodDocumentation parentDocumentation, 
		DiagramNamespaceFilterCriteria methodNamespaces);
}