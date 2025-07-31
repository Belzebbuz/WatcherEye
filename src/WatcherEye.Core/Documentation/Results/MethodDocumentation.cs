using WatcherEye.Core.Documentation.Abstractions;

namespace WatcherEye.Core.Documentation.Results;

public class MethodDocumentation
{
	/// <summary>
	/// Название метода
	/// </summary>
	public required string MethodName { get; init; }
	
	/// <summary>
	/// Тип внутри которого находится метод
	/// </summary>
	public required string ParentTypeName { get; init; }
	
	/// <summary>
	/// Пространство имен
	/// </summary>
	public required string Namespace { get; init; }

	/// <summary>
	/// Перегрузки
	/// </summary>
	public IReadOnlyCollection<OverloadDocumentation> OverloadMethods => _overloads.AsReadOnly();

	private readonly List<OverloadDocumentation> _overloads = new();

	public void AddOverload(OverloadDocumentation document)
	{
		if(_overloads.Any(x => x.OverloadName == document.OverloadName))
			return;
		
		_overloads.Add(document);
	}

	public IEnumerable<T> GetAllDependencies<T>() where T : IDependency
	{
		return _overloads.SelectMany(x => x.Dependencies).OfType<T>();
	}
}