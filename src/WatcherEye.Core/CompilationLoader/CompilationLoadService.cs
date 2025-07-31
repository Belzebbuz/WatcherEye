using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using WatcherEye.Core.CompilationLoader.Abstractions;
using WatcherEye.Core.Exceptions;

namespace WatcherEye.Core.CompilationLoader;

internal class CompilationLoadService(ILogger<CompilationLoadService> logger) : ICompilationLoadService
{
	private readonly Dictionary<string, List<Compilation>> _compilations = [];
	
	public async Task<IReadOnlyList<Compilation>> LoadCompilationAsync(string path)
	{
		if (_compilations.TryGetValue(path, out var compilations))
			return compilations.AsReadOnly();
		_compilations[path] = new List<Compilation>();
		using var workspace = MSBuildWorkspace.Create();
		var solution = await workspace.OpenSolutionAsync(path, new ProjectLoadStatus(logger));
		logger.LogInformation($"Сборка успешно загружена {solution.FilePath}");
		foreach (var project in solution.Projects)
		{
			var compilation = await project.GetCompilationAsync();
			if (compilation == null)
			{
				throw new WatcherEyeCoreException($"Не удалось загрузить компиляцию {project.Name}");
			}
			
			_compilations[path].Add(compilation);
			logger.LogInformation($"Загружена компиляция проекта: {project.Name}");
		}
		return _compilations[path];
	}
}