using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Markdown.Abstractions;

public interface IMarkdownDocumentService
{
	public Task WriteDocumentAsync(DocumentationData documentData, string outputFolder, SolutionPath solutionPath);
}