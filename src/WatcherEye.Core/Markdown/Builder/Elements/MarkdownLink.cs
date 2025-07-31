using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownLink(string value, string href) : IMarkdownLink
{
	public string Value { get; set; } = value;
	public string Href { get; set; } = href;
	public virtual string BuildElement()
	{
		return $"[{Value}]({Href})";
	}
}