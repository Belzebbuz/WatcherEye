using System.Xml.Linq;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Statements;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.Documentation.Results;

/// <summary>
/// Полная информация о перегрузке метода
/// </summary>
public class OverloadDocumentation(IReadOnlyList<ISequenceStatement> statements)
{
	public Guid Id { get; } = Guid.NewGuid();

	public required MethodDocumentation Parent { get; set; }
	/// <summary>
	/// Название метода с типами параметров
	/// </summary>
	public required string OverloadName { get; set; }
	
	/// <summary>
	/// Полное описание сигнатуры
	/// </summary>
	public required string FullSignature { get; set; }
	
	/// <summary>
	/// Xml описание метода
	/// </summary>
	public string? XmlComment { get; init; }
	
	public XDocument? ParsedXmlComment => XmlComment.IsNullOrEmpty() is false ? XDocument.Parse(XmlComment!) : null;
	/// <summary>
	/// Возвращаемый тип
	/// </summary>
	public required ReturnType ReturnType { get; init; }
	
	/// <summary>
	/// Место определения
	/// </summary>
	public required string DeclarationUrl { get; init; }
	
	/// <summary>
	/// Последовательность выражений метода
	/// </summary>
	public IReadOnlyList<ISequenceStatement> Sequence => statements;

	/// <summary>
	/// Зависимости метода
	/// </summary>
	public IReadOnlyCollection<IDependency> Dependencies { get; set; } = [];

	/// <summary>
	/// Коллекция параметров
	/// </summary>
	public required IReadOnlyCollection<MethodParameter> Parameters { get; init; }

	/// <summary>
	/// Коллекция параметров обобщения
	/// </summary>
	public required IReadOnlyCollection<TypeParameter> TypeParameters { get; init; }
}

public class ReturnType
{
	public required string Name { get; init; }
	public required IReadOnlyCollection<TypeParameter> TypeArguments { get; init; }
}