using System.Text;
using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownTableRow : IMarkdownTableRow
{
	private readonly List<IMarkdownLine> _cells = new();
	
	public string BuildElement()
	{
		var stringBuilder = new StringBuilder();
		if (_cells.Count <= 0)
			return "||";
		stringBuilder.Append('|');
		foreach (var cell in _cells)
		{
			stringBuilder.Append(cell.BuildElement());
			stringBuilder.Append('|');
		}
		return stringBuilder.ToString();
	}

	public void Append(IMarkdownLine element)
	{
		_cells.Add(element);
	}

	public void AppendRange(IEnumerable<IMarkdownLine> elements)
	{
		_cells.AddRange(elements);
	}
}