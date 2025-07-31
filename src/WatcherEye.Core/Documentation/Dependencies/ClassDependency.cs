using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using WatcherEye.Core.Comparers;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Results;
using WatcherEye.Core.Extensions;

namespace WatcherEye.Core.Documentation.Dependencies;

/// <summary>
/// Реалиазация зависимости
/// </summary>
public class ClassDependency : IDependency
{
	public Guid Id { get; } = Guid.NewGuid();
	public required string TypeName { get; init; }
	public string? XmlDescription { get; init; }
	public required string TypeDeclarationUrl { get; init; }
	public required string Namespace { get; init; }
	public XDocument? ParsedXmlComment => XmlDescription.IsNullOrEmpty() is false ? XDocument.Parse(XmlDescription!) : null;

	private readonly Dictionary<IMethodSymbol, OverloadDocumentation> _methods = new (SymbolByNameEqualityComparer.Instance);
	public IReadOnlyCollection<OverloadDocumentation> Methods => _methods.Values.ToList().AsReadOnly();
	
	public OverloadDocumentation? MethodExists(IMethodSymbol method) => _methods.GetValueOrDefault(method);
	public void TryAddDocumentation(IMethodSymbol method, OverloadDocumentation documentation) => _methods.TryAdd(method, documentation);
}