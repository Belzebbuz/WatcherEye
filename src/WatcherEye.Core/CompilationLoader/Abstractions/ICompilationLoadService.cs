using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

namespace WatcherEye.Core.CompilationLoader.Abstractions;

public interface ICompilationLoadService
{
	public Task<IReadOnlyList<Compilation>> LoadCompilationAsync(string path);
}

public class ProjectLoadStatus(ILogger logger) : IProgress<ProjectLoadProgress>
{
	public void Report(ProjectLoadProgress value)
	{
		logger.LogDebug($"Загружен проект {value.FilePath}. Время {value.ElapsedTime}ms");
	}
}