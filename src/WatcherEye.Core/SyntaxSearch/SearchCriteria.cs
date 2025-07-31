namespace WatcherEye.Core.SyntaxSearch;

public readonly record struct SearchCriteria(
	string[]? ClassNames,
	string[]? InterfaceNames,
	string[]? MethodName,
	string[]? TypeNamespaces,
	string[]? IncludeDependencyMethodNamespace);

public readonly record struct MethodFilterCriteria(string[]? Names);
public readonly record struct DiagramNamespaceFilterCriteria(string[]? Namespaces);