using Microsoft.Extensions.Logging;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Exceptions;
using WatcherEye.Core.SyntaxSearch;
using WatcherEye.Core.Watcher.Abstractions;

namespace WatcherEye.Core.Documentation;
//TODO в порядке вызовов методов делать ссылку по guid на конкретную строку в таблице если это интерфейс и прямо на метод если реализация
//добавить if else
//добавить создание объектов
internal sealed class DocumentationService(
	ILogger<DocumentationService> logger, 
	ISearchFilter filter,
	IEntryMethodsResolver methodResolver,
	ISolutionWatcherFactory solutionWatcherFactory,
	IDocumentationCollectorFactory  documentationCollectorFactory)
	: IDocumentationService
{
	public async Task<DocumentationData> GenerateAsync(GenerateRequest generateRequest)
	{
		var watcher = await solutionWatcherFactory.CreateAsync(generateRequest.SolutionPath.Value);
		var entryPoints = watcher.FindEntryPoints(generateRequest.Criteria);
		foreach (var entryPoint in entryPoints)
		{
			logger.LogDebug($"Найдена точка входа {entryPoint.NamedTypeSymbol.Name}");
		}

		if (entryPoints.Count == 0)
			throw new WatcherEyeCoreException("Не найдено ни одной точки входа");
		
		var resolvedClassData = methodResolver.GroupMethods(entryPoints, new MethodFilterCriteria(generateRequest.Criteria.MethodName));
		var dataCollector = documentationCollectorFactory.Create(watcher);
		var result = new List<EntryPointDocumentation>();
		foreach (var classData in resolvedClassData)
		{
			//для каждого метода в classData создаем MethodDocumentation
			var methodDocumentations = new List<MethodDocumentation>();
			foreach (var methodData in classData.Methods)
			{
				//для каждой перегрузки в OverloadNodes создаем OverloadDocumentation
				var parentMethod = new MethodDocumentation
				{
					MethodName = methodData.MethodName,
					ParentTypeName = classData.Info.Name,
					Namespace = methodData.Namespace,
				};
				
				foreach (var node in methodData.OverloadNodes)
				{
					var documentation = dataCollector.GetDocumentation(node, classData.Info, parentMethod, new DiagramNamespaceFilterCriteria(generateRequest.Criteria.IncludeDependencyMethodNamespace));
					parentMethod.AddOverload(documentation);
				}
				methodDocumentations.Add(parentMethod);
			}
			//для каждого класса создаем свой EntryPointGenerationResult
			var entryPointDoc = new EntryPointDocumentation(classData.Info.Name,
				classData.Info.FullName,
				classData.Info.XmlComment,
				methodDocumentations.AsReadOnly());
			result.Add(entryPointDoc);
		}

		return new DocumentationData(result);
	}
}