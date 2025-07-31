namespace WatcherEye.Core.Markdown.Builder.Abstractions;

public interface IMermaidDiagram : IMarkdownElement
{
	void Append(IMermaidSequencePart element);
}