using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Documentation.Statements;
using WatcherEye.Core.Exceptions;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class OverloadDocumentationWalker(OverloadDocumentationContext context) : CSharpSyntaxWalker
{
	public override void VisitInvocationExpression(InvocationExpressionSyntax node)
	{
		var isMethodExpression = node.Expression.TryCast<MemberAccessExpressionSyntax>(out var expression);
		var isMethodSymbol = context.SemanticModel.GetSymbolInfo(node).Symbol
			.TryCast<IMethodSymbol>(out var externalMethodSymbol);
		var isFilterContains = context.SearchFilter.ContainsDependencyMethodByNamespace(externalMethodSymbol,
			context.DiagramInvocationNamespaces);
		if (!isMethodExpression || !isMethodSymbol || !isFilterContains)
		{
			base.VisitInvocationExpression(node);
			return;
		}
		var externalType = context.SemanticModel.GetTypeInfo(expression.Expression).Type;
		if (externalType is null)
		{
			base.VisitInvocationExpression(node);
			return;
		}
		var overloadExternalDocumentation = GetExternalMethodDocumentation(externalMethodSymbol, externalType);
		var statement = PrepareInvocationStatement(node, expression, overloadExternalDocumentation, externalType);
		context.StatementCollector.PushStatement(statement);
		base.VisitInvocationExpression(node);
		context.StatementCollector.FillStackStatements();
	}

	private IReadOnlyCollection<OverloadDocumentation> GetExternalMethodDocumentation(
		IMethodSymbol externalMethodSymbol,
		ITypeSymbol externalType)
	{
		var targetFakeSyntax = externalType.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
		var result = context.DependencyCollector.Collect(context, externalMethodSymbol, targetFakeSyntax);
		return result;
	}

	private InvocationStatement PrepareInvocationStatement(InvocationExpressionSyntax node,
		MemberAccessExpressionSyntax expression,
		IReadOnlyCollection<OverloadDocumentation> overloadExternalDocumentation,
		ITypeSymbol externalType)
	{
		var methodName = expression.Name.Identifier.ToFullString();
		var returnType = context.SemanticModel.GetTypeInfo(node);
		var typeName = returnType.Type?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
		               ?? throw new WatcherEyeCoreException(
			               $"Не удалось получить имя возвращаемого типа: {node.ToString()}");
		var targetTypeName = externalType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
		                     ?? throw new WatcherEyeCoreException($"Не удалось получить имя типа: {node.ToString()}");
		var statement = new InvocationStatement
		{
			TargetTypeName = targetTypeName,
			SourceType =
				context.ClassDeclarationInfo.NamedTypeSymbol.ToDisplayString(SymbolDisplayFormat
					.MinimallyQualifiedFormat),
			InvocationName = methodName,
			ReturnType = typeName,
			DeclarationUrl = expression.GetPath(),
			ExternalDocumentation = overloadExternalDocumentation,
		};
		return statement;
	}

	public override void VisitIdentifierName(IdentifierNameSyntax node)
	{
		if (node.Parent is not InvocationExpressionSyntax) // если это не метод - выходим
		{
			base.VisitIdentifierName(node);
			return;
		}

		base.VisitIdentifierName(node);
		context.StatementCollector.FillStackStatements();
		
		if (context.SemanticModel.GetSymbolInfo(node).Symbol
			    ?.DeclaringSyntaxReferences.FirstOrDefault()
			    ?.GetSyntax() is not MethodDeclarationSyntax localMethodCallSyntax)
			return;
		var isLocalyDeclarated = context.ClassDeclarationInfo.Declaration.Contains(localMethodCallSyntax);
		//Если метод не определен локально (унаследован), нужно найти определение и пройтись по нему
		if (!isLocalyDeclarated)
		{
			ClassDeclarationSyntax? baseSyntax = null;
			INamedTypeSymbol? baseType = context.ClassDeclarationInfo.NamedTypeSymbol.BaseType;
			//Ищем базовый класс в котором определен метод
			while (baseType is not null)
			{
				baseSyntax = baseType.DeclaringSyntaxReferences.FirstOrDefault()
					?.GetSyntax() as ClassDeclarationSyntax;
				var methodContains = baseSyntax?.Contains(localMethodCallSyntax);
				if (methodContains is true && baseSyntax is not null)
					break;
				baseType = baseType.BaseType;
			}

			if (baseSyntax is not null)
			{
				var baseTypeDeclarationInfo =
					context.Watcher.FindDeclarationsInfo(baseSyntax);
				var methodSymbol = baseTypeDeclarationInfo?.SemanticModel.GetDeclaredSymbol(localMethodCallSyntax);
				if (baseTypeDeclarationInfo is not null && methodSymbol is not null)
				{
					var newContext = new OverloadDocumentationContext
					{
						ClassDeclarationInfo = baseTypeDeclarationInfo,
						RootSyntax = localMethodCallSyntax,
						RootMethodSymbol = methodSymbol,
						StatementCollector = context.StatementCollector,
						SearchFilter = context.SearchFilter,
						DependencyCollector = context.DependencyCollector,
						Watcher = context.Watcher,
						ParentDocumentation = context.ParentDocumentation,
						DiagramInvocationNamespaces = context.DiagramInvocationNamespaces,
					};
					
					var walker = new OverloadDocumentationWalker(newContext);
					walker.VisitMethodDeclaration(context.RootSyntax);
					var documentation = context.BuildDocumentation();
					foreach (var sequenceStatement in documentation.Sequence)
					{
						context.StatementCollector.AddStatement(sequenceStatement);
					}

					return;
				}
			}
		}

		base.VisitMethodDeclaration(localMethodCallSyntax);
	}

	public override void VisitThrowStatement(ThrowStatementSyntax node)
	{
		var open = new OpenThrowStatement();
		context.StatementCollector.AddStatement(open);

		base.VisitThrowStatement(node);

		var close = new CloseThrowStatement
		{
			OpenStatement = open
		};
		context.StatementCollector.AddStatement(close);
		open.CloseStatement = close;
	}

	public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
	{
		var openStatement = new OpenObjectCreationStatement
		{
			ObjectType = node.Type.ToFullString()
		};
		context.StatementCollector.AddStatement(openStatement);

		base.VisitObjectCreationExpression(node);

		var closeStatement = new CloseObjectCreationStatement
		{
			OpenStatement = openStatement
		};
		openStatement.CloseStatement = closeStatement;
		context.StatementCollector.AddStatement(closeStatement);
	}

	public override void VisitIfStatement(IfStatementSyntax node)
	{
		var openStatement = new OpenIfStatement
		{
			Condition = node.Condition.ToFullString()
		};
		context.StatementCollector.AddStatement(openStatement);

		base.VisitIfStatement(node);
		var closeStatement = new CloseIfStatement
		{
			OpenStatement = openStatement
		};
		openStatement.CloseStatement = closeStatement;
		context.StatementCollector.AddStatement(closeStatement);
	}

	public override void VisitElseClause(ElseClauseSyntax node)
	{
		var openStatement = new OpenElseStatement();
		context.StatementCollector.AddStatement(openStatement);

		base.VisitElseClause(node);
		var closeStatement = new CloseElseStatement()
		{
			OpenStatement = openStatement
		};
		openStatement.CloseStatement = closeStatement;
		context.StatementCollector.AddStatement(closeStatement);
	}
}