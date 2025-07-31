namespace WatcherEye.Core.Documentation.Results;

public record EntryPointDocumentation(string ClassName, string FullClassName, string? XmlComment, IReadOnlyCollection<MethodDocumentation> Methods);