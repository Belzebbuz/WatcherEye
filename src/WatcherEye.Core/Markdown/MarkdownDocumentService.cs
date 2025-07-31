using Microsoft.Extensions.Logging;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Exceptions;
using WatcherEye.Core.Markdown.Abstractions;

namespace WatcherEye.Core.Markdown;

public class MarkdownDocumentService(ILogger<MarkdownDocumentService> logger) : IMarkdownDocumentService
{
	private const string RootFolder = "docs";
	private SolutionPath? _solutionPath;
	public async Task WriteDocumentAsync(DocumentationData documentData, string outputFolder, SolutionPath solutionPath)
	{
		_solutionPath = solutionPath;
		var rootFolder = CombineAnCreateDirectory(outputFolder, RootFolder);
		var addedFolders = new HashSet<string>();
		foreach (var entryPoint in documentData.EntryPoints)
		{
			var newFolderName = addedFolders.Add(entryPoint.ClassName) ? entryPoint.ClassName : entryPoint.FullClassName;
			var entryPointFolder = CombineAnCreateDirectory(rootFolder, newFolderName);
			await HandleEntryPoint(entryPoint, entryPointFolder);
		}
	}

	private async Task HandleEntryPoint(EntryPointDocumentation entryPoint, string entryPointFolder)
	{
		var entryPointPath = Path.Combine(entryPointFolder, $"{entryPoint.ClassName}.md");
		await WriteEntryPointDoc(entryPoint, entryPointPath);
		foreach (var method in entryPoint.Methods)
		{
			var markdownFilePath = Path.Combine(entryPointFolder, $"{method.MethodName}.md");
			await WriteMethodDoc(method, markdownFilePath);
		}
	}

	//пример https://learn.microsoft.com/ru-ru/dotnet/api/system.net.http.httpclient?view=net-8.0
	//описание класса
	private Task WriteEntryPointDoc(EntryPointDocumentation entryPoint, string entryPointPath)
	{
		return Task.CompletedTask;
	}

	private async Task WriteMethodDoc(MethodDocumentation method, string markdownFilePath)
	{
		if(_solutionPath == null)
			return;
		logger.LogInformation($"Начал формирование {markdownFilePath}");
		var markdownWalker = new MarkdownMethodDocsWalker(_solutionPath);
		var markdown = markdownWalker.BuildMarkdown(method);
		await File.WriteAllTextAsync(markdownFilePath, markdown);
		logger.LogInformation($"Закончил формирование {markdownFilePath}");
	}

	private static string CombineAnCreateDirectory(string outputFolder,string newFolder)
	{
		if (!Directory.Exists(outputFolder))
			throw new WatcherEyeCoreException($"Директория не найдена {outputFolder}");
		var rootFolder = Path.Combine(outputFolder, newFolder);
		if (!Directory.Exists(rootFolder)) 
			Directory.CreateDirectory(rootFolder);
		
		return rootFolder;
	}
}