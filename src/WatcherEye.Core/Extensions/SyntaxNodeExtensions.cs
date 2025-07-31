using Microsoft.CodeAnalysis;

namespace WatcherEye.Core.Extensions;

public static class SyntaxNodeExtensions
{
	public static string GetPath(this SyntaxNode node)
	{
		var location = node.GetLocation();
		var locationLine = location.GetLineSpan();
		return $"{location.SourceTree?.FilePath}#L{locationLine.StartLinePosition.Line + 1}";
	}
}