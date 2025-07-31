using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownCodeBlock(string code, string language) : IMarkdownCodeBlock
{
	public string Code { get; set; } = code;
	public string Language { get; set; } = language;

	private const string Format = """
	                              ```{0}
	                              {1}
	                              ```
	                              """;
	public string BuildElement()
	{
		return string.Format(Format, Language, Code);
	}
}