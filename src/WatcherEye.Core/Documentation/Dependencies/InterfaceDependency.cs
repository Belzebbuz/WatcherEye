using System.Xml.Linq;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.Documentation.Dependencies;

/// <summary>
/// Абстракция зависимости с набором реализаций
/// </summary>
public class InterfaceDependency : IDependency
{
	public Guid Id { get; } = Guid.NewGuid();
	public required string TypeName { get; init; }
	public string? XmlDescription { get; init; }
	public XDocument? ParsedXmlComment => XmlDescription.IsNullOrEmpty() is false ? XDocument.Parse(XmlDescription!) : null;
	public required string TypeDeclarationUrl { get; init; }
	public required string Namespace { get; init; }

	private readonly List<ClassDependency> _classDependencies = new();
	public required IReadOnlyCollection<ClassDependency> Implementations
	{
		get => _classDependencies.AsReadOnly();
		init
		{
			_classDependencies.Clear();
			_classDependencies.AddRange(value);
		}
	}

	public void AddDependencies(IReadOnlyCollection<ClassDependency> dependencies)
	{
		var newDependencies = dependencies.ExceptBy(_classDependencies.Select(x => x.Id), dependency => dependency.Id);
		_classDependencies.AddRange(newDependencies);
	}
}