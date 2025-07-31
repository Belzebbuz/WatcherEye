using System.Text;
using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownTable: IMarkdownTable
{
	private readonly List<IMarkdownTableRow> _rows = new();

	public void Append(IMarkdownTableRow element)
	{
		_rows.Add(element);
	}
	
	public string BuildElement()
	{
		var stringBuilder = new StringBuilder();
		foreach (var row in _rows)
		{
			stringBuilder.AppendLine(row.BuildElement());
		}
		return stringBuilder.ToString();
	}

	public IMarkdownTableRow AddRow()
	{
		var row = new MarkdownTableRow();
		_rows.Add(row);
		return row;
	}

	public void AddRowIfNotExists(IMarkdownTableRow row)
	{
		var newRow = row.BuildElement();
		if(_rows.Any(x => x.BuildElement() == newRow))
			return;
		_rows.Add(row);
	}
}