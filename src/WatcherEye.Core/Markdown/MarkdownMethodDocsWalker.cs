using System.Collections.Immutable;
using System.Xml.Linq;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Dependencies;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Documentation.Statements;
using WatcherEye.Core.Extensions;
using WatcherEye.Core.Markdown.Builder;
using WatcherEye.Core.Markdown.Builder.Elements;
using WatcherEye.Core.Markdown.Builder.Elements.Mermaid;

namespace WatcherEye.Core.Markdown;

public class MarkdownMethodDocsWalker(SolutionPath solutionPath)
{
	private readonly MarkdownBuilder _builder = new();
	public string BuildMarkdown(MethodDocumentation methodDocumentation)
	{
		_builder.Clear();
		Visit(methodDocumentation);
		return _builder.Build();
	}

	public void Visit(MethodDocumentation document)
	{
		_builder.AddHeader($"{document.ParentTypeName}.{document.MethodName} Метод", 1);
		_builder.AddHeader("Определение", 2);

		var namespaceRow = new MarkdownText($"Пространство имен: {document.Namespace}");
		_builder.AddLine(namespaceRow);

		VisitOverloads(document.OverloadMethods);
		
		
		AddDependencyDocumentation(document);
	}

	private void AddDependencyDocumentation(MethodDocumentation document)
	{
		var interfacesDependencies = document.GetAllDependencies<InterfaceDependency>();
		var classDependencies = document.GetAllDependencies<ClassDependency>();

		AddInterfaceDependencies(interfacesDependencies);
		AddClassDependencies(classDependencies);
	}

	private void AddClassDependencies(IEnumerable<ClassDependency> classDependencies)
	{
		foreach (var classDependency in classDependencies)
		{
			_builder.AddHeader(classDependency.TypeName, 1);
			_builder.AddLine(new MarkdownText($"Пространство имен: {classDependency.Namespace}"));
			var sourceCodeLine = _builder.AddLine(new MarkdownText("Исходный код: "));
			sourceCodeLine.Append(new MarkdownLink(classDependency.TypeName, solutionPath.GetRemotePath(classDependency.TypeDeclarationUrl)));
			_builder.AddLine(new MarkdownText(classDependency.ParsedXmlComment?.GetSummary() ?? string.Empty));
			foreach (var method in classDependency.Methods)
			{
				VisitOverload(method);
			}
		}
	}

	private void AddInterfaceDependencies(IEnumerable<InterfaceDependency> interfacesDependencies)
	{
		foreach (var interfaceDependency in interfacesDependencies.OrderBy(x => x.TypeName))
		{
			_builder.AddHeader(interfaceDependency.TypeName, 1);
			_builder.AddLine(new MarkdownText($"Пространство имен: {interfaceDependency.Namespace}"));
			var sourceCodeLine = _builder.AddLine(new MarkdownText("Исходный код: "));
			sourceCodeLine.Append(new MarkdownLink(interfaceDependency.TypeName, solutionPath.GetRemotePath(interfaceDependency.TypeDeclarationUrl)));
			_builder.AddLine(new MarkdownText(interfaceDependency.ParsedXmlComment?.GetSummary() ?? string.Empty));
			_builder.AddHeader("Реализации", 2);
			var table = _builder.AddTable(3);
			foreach (var implementation in interfaceDependency.Implementations.OrderBy(x => x.TypeName))
			{
				var className = new MarkdownLine();
				className.Append(new MarkdownAnchorLink(implementation.TypeName, implementation.TypeName));
				foreach (var overloadDocumentation in implementation.Methods)
				{
					var row = table.AddRow();
					var overloadName = new MarkdownLine();
					overloadName.Append(new MarkdownAnchorLink(overloadDocumentation.OverloadName, overloadDocumentation.Id.ToString()));
					var comment = new MarkdownLine();
					comment.Append(overloadDocumentation.ParsedXmlComment?.GetSummary() ?? string.Empty);
					row.Append(className);
					row.Append(overloadName);
					row.Append(comment);
				}

				_builder.AddHeader(implementation.TypeName, 1);
				foreach (var overloadDocumentation in implementation.Methods)
				{
					VisitOverload(overloadDocumentation);
				}
			}
		}
	}

	private void VisitOverloads(IReadOnlyCollection<OverloadDocumentation> documentOverloads)
	{
		_builder.AddHeader("Перегрузки", headerLevel: 2);
		var table = _builder.AddTable(columnCount: 2);
		foreach (var overload in documentOverloads)
		{
			var row = table.AddRow();
			var nameCell = new MarkdownLine();
			nameCell.Append(new MarkdownAnchorLink(overload.OverloadName, overload.Id.ToString()));
			var commentCell = new MarkdownLine();
			commentCell.Append(new MarkdownText(overload.ParsedXmlComment.GetSummary() ?? "Описание отсутствует"));

			row.Append(nameCell);
			row.Append(commentCell);
		}

		foreach (var overload in documentOverloads)
		{
			VisitOverload(overload);
		}
	}

	
	private void VisitOverload(OverloadDocumentation documentOverload)
	{
		_builder.AddHeader($"<a id={documentOverload.Id}></a> {documentOverload.OverloadName}", 2);
		var link = new MarkdownLink(documentOverload.OverloadName, solutionPath.GetRemotePath(documentOverload.DeclarationUrl));
		var line = _builder.AddLine(new MarkdownText("Исходный код: "));
		line.Append(link);
		XDocument? comment = null;
		if (!documentOverload.XmlComment.IsNullOrEmpty())
		{
			comment = XDocument.Parse(documentOverload.XmlComment!);
			var summary = comment.GetSummary();
			if (!summary.IsNullOrEmpty())
			{
				var summaryText = new MarkdownText(summary!);
				_builder.AddLine(summaryText);
			}
		}

		_builder.AddCodeBlock(documentOverload.FullSignature);

		AddParameters(documentOverload, comment);

		AddReturnType(documentOverload, comment);
		
		//TODO исключения
		//TODO примеры кода

		AddDependencyList(documentOverload);
		
		AddDependencyDiagram(documentOverload);
	}

	private void AddParameters(OverloadDocumentation documentOverload, XDocument? comment)
	{
		if (documentOverload.Parameters.Count > 0)
		{
			var paramComment = comment?.Root?.Elements("param")
				.Select(x => new { Name = x.Attribute("name")?.Value, x.Value })
				.ToArray();
			_builder.AddHeader("Параметры", 3);
			foreach (var parameter in documentOverload.Parameters)
			{
				var parameterText = new MarkdownFormattedText(parameter.Name, TextFormat.Code);
				var parameterLine = _builder.AddLine(parameterText);
				parameterLine.Append(" - ");
				if (parameter.DeclarationUrl.IsNullOrEmpty())
					parameterLine.Append(new MarkdownFormattedText(parameter.Type, TextFormat.Code));
				else
					parameterLine.Append(new MarkdownLink(parameter.Type, solutionPath.GetRemotePath(parameter.DeclarationUrl!)));

				var parameterComment = paramComment?.FirstOrDefault(x => x.Name == parameter.Name)?.Value;
				if (!parameterComment.IsNullOrEmpty())
				{
					var parameterCommentText = new MarkdownText(parameterComment!);
					_builder.AddLine(parameterCommentText);
				}
			}
		}
	}

	private void AddReturnType(OverloadDocumentation documentOverload, XDocument? comment)
	{
		if (documentOverload.ReturnType.Name != "void")
		{
			_builder.AddHeader("Возвращаемое значение", 3);
			_builder.AddLine(new MarkdownFormattedText(documentOverload.ReturnType.Name, TextFormat.Code));
			var returnComment = comment?.Root?.Element("returns")?.Value.Trim();
			if (!returnComment.IsNullOrEmpty())
				_builder.AddLine(new MarkdownText(returnComment!));
		}
	}

	private void AddDependencyList(OverloadDocumentation documentOverload)
	{
		var dependencies = documentOverload.Sequence
			.OfType<InvocationStatement>()
			.Where(x => x.ExternalDocumentation?.Count > 0).ToImmutableArray();
		if (dependencies.Length > 0)
		{
			_builder.AddHeader("Порядок вызова зависимостей", 3);
			foreach (var dependency in dependencies)
			{
				var fullName = $"{dependency.TargetTypeName}.{dependency.InvocationName}";
				var anchorLink = new MarkdownAnchorLink(fullName, dependency.TargetTypeName);
				var line = _builder.AddLine(anchorLink);
				line.Append(" - ");
				line.Append(new MarkdownLink("Исходный код", solutionPath.GetRemotePath(dependency.DeclarationUrl)));
			}
		}
	}

	private void AddDependencyDiagram(OverloadDocumentation documentOverload)
	{
		if (documentOverload.Sequence.Count > 0)
		{
			_builder.AddHeader("Диаграмма вызовов", 3);
			var diagram = _builder.AddSequenceDiagram();
			var clientCall = new SequenceInvocationCall("client",
				documentOverload.Parent.ParentTypeName, documentOverload.OverloadName, false);
			diagram.Append(clientCall);
			foreach (var sequence in documentOverload.Sequence)
			{
				if (sequence is InvocationStatement invocationStatement)
				{
					var callStatement = new SequenceInvocationCall(invocationStatement.SourceType,
						invocationStatement.TargetTypeName, invocationStatement.InvocationName, false);
					diagram.Append(callStatement);
					if(invocationStatement.ReturnType.IsNullOrEmpty() || invocationStatement.ReturnType == "void" || invocationStatement.ReturnType == invocationStatement.SourceType)
						continue;
					var responseStatement = new SequenceInvocationCall(invocationStatement.TargetTypeName,
						invocationStatement.SourceType, invocationStatement.ReturnType, true);
					diagram.Append(responseStatement);
				}
			}
			
			var clientResponse = new SequenceInvocationCall(documentOverload.Parent.ParentTypeName,
				"client", documentOverload.ReturnType.Name, true);
			diagram.Append(clientResponse);
		}
	}
}