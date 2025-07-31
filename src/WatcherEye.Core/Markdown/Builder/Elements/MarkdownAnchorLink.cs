namespace WatcherEye.Core.Markdown.Builder.Elements;

public class MarkdownAnchorLink(string value, string href) : MarkdownLink(value, href)
{
	public override string BuildElement()
	{
		var formattedRef = Href.Replace(".",string.Empty).Replace("(",string.Empty).Replace(")", string.Empty).Replace(", ", string.Empty).ToLower();
		return $"[{Value}](#{formattedRef})";
	}
}