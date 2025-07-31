using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements;

public class HeaderElement : IMarkdownHeader
{
	private int _level;

	public required int Level
	{
		get => _level;
		set
		{
			if (value is < 1 or > 6) 
				throw new ArgumentException("Допустимые уровни заголовка 1-6");
			_level = value;
		}
	}

	public required string Value { get; set; }

	public string BuildElement()
	{
		var size = new string('#', Level);
		return $"{size} {Value}";
	}
}