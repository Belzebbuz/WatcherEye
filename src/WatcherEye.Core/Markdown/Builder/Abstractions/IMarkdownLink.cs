namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownLink : IMarkdownText
{
	public string Href { get; set; }
}