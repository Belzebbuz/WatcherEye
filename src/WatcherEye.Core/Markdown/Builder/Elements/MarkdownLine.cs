using System.Text;
using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownLine : IMarkdownLine
{
	private readonly List<IMarkdownText> _lineParts = new();
	public IReadOnlyCollection<IMarkdownText> LineParts => _lineParts.AsReadOnly();
	

	public bool IsNewline { get; set; }
	public void Append(IMarkdownText element)
	{
		_lineParts.Add(element);
	}
	public void Append(MarkdownText element)
	{
		_lineParts.Add(element);
	}
	public string BuildElement()
	{
		var stringBuilder = new StringBuilder();
		foreach (var element in LineParts)
		{
			stringBuilder.Append(element.BuildElement());
		}
		if(IsNewline)
			stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

}