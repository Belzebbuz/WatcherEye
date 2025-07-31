namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownText : IMarkdownElement
{
	public string Value { get; set; }
}