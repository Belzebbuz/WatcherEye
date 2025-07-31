using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownText(string value) : IMarkdownText
{
	public static implicit operator MarkdownText(string value) => new(value);
	public string Value { get; set; } = value;
	
	public virtual string BuildElement()
	{
		return Value;
	}
}