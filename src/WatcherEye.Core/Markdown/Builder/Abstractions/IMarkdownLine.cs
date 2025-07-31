using WatcherEye.Core.Markdown.Builder.Elements;

namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownLine : IMarkdownElement
{
	public IReadOnlyCollection<IMarkdownText> LineParts { get; }
	public void Append(IMarkdownText element);
	public void Append(MarkdownText element);
	public bool IsNewline { get; set; }
}