namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownFormattedText(string value, TextFormat textFormat) : MarkdownText(value)
{
	public override string BuildElement()
	{
		if (textFormat.HasFlag(TextFormat.Code))
		{
			return $"`{Value}`";
		}
		
		var format = "{0}";
		if (textFormat.HasFlag(TextFormat.Bold))
		{
			format = $"**{format}**";
		}

		if (textFormat.HasFlag(TextFormat.Italic))
		{
			format = $"*{format}*";
		}
		
		return string.Format(format, Value);
	}
}