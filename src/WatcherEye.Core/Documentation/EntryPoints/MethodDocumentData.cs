using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WatcherEye.Core.Documentation.EntryPoints;

/// <summary>
/// Информация о методе и его перегрузках 
/// </summary>
public record MethodDocumentData(string MethodName, string Namespace, IReadOnlyCollection<MethodDeclarationSyntax> OverloadNodes);