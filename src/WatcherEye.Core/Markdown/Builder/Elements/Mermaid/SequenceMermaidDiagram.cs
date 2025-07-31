using System.Text;
using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements.Mermaid;

public class SequenceMermaidDiagram : IMermaidDiagram
{
	private const string Format = """
	                              ```mermaid
	                              sequenceDiagram
	                              autonumber
	                              {0}
	                              ```
	                              """;
	private readonly List<IMermaidSequencePart> _sequenceParts = new();

	public void Append(IMermaidSequencePart element)
	{
		_sequenceParts.Add(element);
	}
	public string BuildElement()
	{
		var builder = new StringBuilder();
		foreach (var part in _sequenceParts)
		{
			builder.AppendLine(part.BuildElement());
		}
		return string.Format(Format, builder);
	}
}