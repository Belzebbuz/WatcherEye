using System.Text;
using WatcherEye.Core.Exceptions;
using WatcherEye.Core.Markdown.Builder.Abstractions;
using WatcherEye.Core.Markdown.Builder.Elements;
using WatcherEye.Core.Markdown.Builder.Elements.Mermaid;

namespace WatcherEye.Core.Markdown.Builder;

public class MarkdownBuilder
{
	private readonly List<IMarkdownElement> _elements = new();
	public void Clear() => _elements.Clear();
	public string Build()
	{
		StringBuilder builder = new();
		builder.Clear();
		foreach (var element in _elements)
		{
			builder.AppendLine(element.BuildElement());
		}
		return builder.ToString();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="header"></param>
	/// <param name="headerLevel"></param>
	/// <returns></returns>
	/// <exception cref="WatcherEyeCoreException"></exception>
	public IMarkdownHeader AddHeader(string header, int headerLevel)
	{
		var headerElement = new HeaderElement
		{
			Level = headerLevel,
			Value = header
		};
		_elements.Add(headerElement);
		return headerElement;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="element"></param>
	/// <param name="newLine"></param>
	/// <returns></returns>
	public IMarkdownLine AddLine(IMarkdownText element, bool newLine = true)
	{
		var line = new MarkdownLine
		{
			IsNewline = newLine
		};
		line.Append(element);
		_elements.Add(line);
		return line;
	}

	public IMarkdownTable AddTable(int columnCount, bool isEmptyHeader = true)
	{
		var table = new MarkdownTable();
		_elements.Add(table);
		if (columnCount >= 0 && isEmptyHeader)
		{
			var headerRow = table.AddRow();
			var nameCells = Enumerable.Repeat(string.Empty, columnCount)
				.Select(x =>
				{
					var line = new MarkdownLine();
					line.Append(new MarkdownText(x));
					return line;
				});
			headerRow.AppendRange(nameCells);
			var divideRow = table.AddRow();
			var divideCells = Enumerable.Repeat("---", columnCount)
				.Select(x =>
				{
					var line = new MarkdownLine();
					line.Append(new MarkdownText(x));
					return line;
				});
			divideRow.AppendRange(divideCells);
		}
		
		return table;
	}

	public IMarkdownCodeBlock AddCodeBlock(string code, string language = "csharp")
	{
		var codeBlock = new MarkdownCodeBlock(code, language);
		_elements.Add(codeBlock);
		return codeBlock;
	}

	public IMermaidDiagram AddSequenceDiagram()
	{
		var diagram = new SequenceMermaidDiagram();
		_elements.Add(diagram);
		return diagram;
	}
}