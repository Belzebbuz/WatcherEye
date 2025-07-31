using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Documentation.Abstractions;

public interface IDocumentationService
{
	public Task<DocumentationData> GenerateAsync(GenerateRequest generateRequest); 
}