using System.Xml.Linq;

namespace WatcherEye.Core.Extensions;

public static class XDocumentExtensions
{
	public static string? GetSummary(this XDocument? document)
	{
		return document?.Root?.Element("summary")?.Value.Trim();
	}
}