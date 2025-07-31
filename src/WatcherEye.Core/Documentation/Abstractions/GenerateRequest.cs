using WatcherEye.Core.Extensions;
using WatcherEye.Core.SyntaxSearch;

namespace WatcherEye.Core.Documentation.Abstractions;

public readonly record struct GenerateRequest(SolutionPath SolutionPath, SearchCriteria Criteria);

public record SolutionPath(string Value, string? RemoteRepositoryUrl)
{
	public string GetRemotePath(string declarationPath)
	{
		if(RemoteRepositoryUrl.IsNullOrEmpty())
			return string.Empty;
		var directory = Path.GetDirectoryName(Value);
		if(directory == null)
			return string.Empty;
		return declarationPath.Replace(directory, RemoteRepositoryUrl).Replace('\\','/');
	}
}