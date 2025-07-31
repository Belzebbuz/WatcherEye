namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownHeader : IMarkdownElement
{
	public int Level { get; set; }
	public string Value { get; set; }
}