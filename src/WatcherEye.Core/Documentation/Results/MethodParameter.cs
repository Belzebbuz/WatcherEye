namespace WatcherEye.Core.Documentation.Results;

public record struct MethodParameter(string Type, string Name, string? DeclarationUrl);
public record struct TypeParameter(string Type, string? Url);