namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownTableRow : IMarkdownElement
{
	public void Append(IMarkdownLine element);
	void AppendRange(IEnumerable<IMarkdownLine> elements);
}