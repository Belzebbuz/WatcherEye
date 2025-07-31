namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownCodeBlock : IMarkdownElement
{
	public string Code { get; set; }
	public string Language { get; set; }
}