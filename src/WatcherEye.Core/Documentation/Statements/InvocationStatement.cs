using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Documentation.Statements;

/// <summary>
/// Выражение вызова метода/свойства у другого типа
/// </summary>
public class InvocationStatement : ISequenceStatement
{
	/// <summary>
	/// Источник вызова
	/// </summary>
	public required string TargetTypeName { get; init; }
	
	/// <summary>
	/// Тип, у которого происходит вызов
	/// </summary>
	public required string SourceType { get; init; }
	
	/// <summary>
	/// Имя свойства или метода
	/// </summary>
	public required string InvocationName { get; init; }
	
	/// <summary>
	/// Возвращаемый тип данных
	/// </summary>
	public required string ReturnType { get; init; }

	/// <summary>
	/// Место в коде
	/// </summary>
	public required string DeclarationUrl { get; set; }
	/// <summary>
	/// Если в решении есть реализация этого метода, то сюда нужно добавить документацию(-ии если их несколько) на него
	/// </summary>
	public IReadOnlyCollection<OverloadDocumentation>? ExternalDocumentation { get; set; }
}