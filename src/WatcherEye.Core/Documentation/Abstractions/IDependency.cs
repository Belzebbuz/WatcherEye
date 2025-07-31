using System.Xml.Linq;

namespace WatcherEye.Core.Documentation.Abstractions;

/// <summary>
/// Зависимость метода
/// </summary>
public interface IDependency
{
	public Guid Id { get; }
	/// <summary>
	/// Имя типа зависимости
	/// </summary>
	public string TypeName { get; init; }
	
	/// <summary>
	/// Xml описание типа
	/// </summary>
	public string? XmlDescription { get; init; }

	public string TypeDeclarationUrl { get; init; }
	public string Namespace { get; init; }
	public XDocument? ParsedXmlComment { get; }
}