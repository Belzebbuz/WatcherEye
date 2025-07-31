namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMarkdownTable : IMarkdownElement
{
	IMarkdownTableRow AddRow();
	void AddRowIfNotExists(IMarkdownTableRow row);
}