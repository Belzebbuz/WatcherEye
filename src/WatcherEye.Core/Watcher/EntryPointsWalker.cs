using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.SyntaxSearch;

namespace WatcherEye.Core.Watcher;

internal class EntryPointsWalker(SemanticModel model, SearchCriteria criteria, ISearchFilter filter)
	: CSharpSyntaxWalker
{
	private readonly List<ClassDeclarationInfo> _implementations = [];
	public IReadOnlyList<ClassDeclarationInfo> Implementations => _implementations.AsReadOnly();
	public override void VisitClassDeclaration(ClassDeclarationSyntax node)
	{
		var classSymbol = model.GetDeclaredSymbol(node);
		if (classSymbol is null || classSymbol.IsAbstract)
		{
			base.VisitClassDeclaration(node);
			return;
		}

		var info = new ClassDeclarationInfo(node, model);
		if (!filter.Contains(classSymbol, in criteria))
		{
			base.VisitClassDeclaration(node);
			return;
		}
		
		_implementations.Add(info);
		base.VisitClassDeclaration(node);
	}
}