using Spectre.Console;
using Spectre.Console.Cli;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Markdown.Abstractions;
using WatcherEye.Core.SyntaxSearch;

namespace WatcherEye.Console.CommandHandlers.Default;

public class DefaultCommand(IDocumentationService documentation, IMarkdownDocumentService markdownDocumentService) : AsyncCommand<DefaultCommandSettings>
{
	public override async Task<int> ExecuteAsync(CommandContext context, DefaultCommandSettings settings)
	{
		if (!string.IsNullOrEmpty(settings.OutputFolder) && !Directory.Exists(settings.OutputFolder))
		{
			AnsiConsole.WriteLine($"Указанной директории не существует {settings.OutputFolder}");
			return 1;
		}
		else
		{
			settings.OutputFolder ??= Directory.GetCurrentDirectory();
		}
		if(string.IsNullOrEmpty(settings.SolutionPath))
			settings.SolutionPath = Directory
				.GetFiles(Directory.GetCurrentDirectory(), "*.sln", SearchOption.AllDirectories)
				.First();
		AnsiConsole.WriteLine($"Начинаю построение документации для");
		DocumentationData? info = null;
		var solutionPath = new SolutionPath(settings.SolutionPath, settings.RemoteRepositoryBasePath);
		await AnsiConsole.Status()
			.StartAsync(("Анализирую..."), async ctx =>
			{
				var filter = new SearchCriteria(settings.ClassName,
					settings.InterfaceName,
					settings.MethodName,
					settings.TypeNamespaces,
					settings.IncludeDependencyMethodNamespace);
				var request = new GenerateRequest(solutionPath, filter);
				info = await documentation.GenerateAsync(request);
				AnsiConsole.WriteLine("Документация проанализирована");
			});
		
		if (info == null)
		{
			AnsiConsole.WriteLine("Не удалось построить документацию, включите подробные логи и изучите в чем может быть проблема");
			return 1;
		}
		
		await AnsiConsole.Status()
			.StartAsync(("Формирую markdown документацию..."), async ctx =>
			{
				await markdownDocumentService.WriteDocumentAsync(info, settings.OutputFolder, solutionPath);
				AnsiConsole.WriteLine("Morkdown документация сформирована");
			});
		return 0;
	}
}