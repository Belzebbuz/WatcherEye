using WatcherEye.Core.Markdown.Builder.Abstractions;

namespace WatcherEye.Core.Markdown.Builder.Elements.Mermaid;

public class SequenceInvocationCall(string source, string target, string invocationComment, bool isResponse) : IMermaidSequencePart
{
	public string BuildElement()
	{
		var formattedSource = source.Replace("<", "[").Replace(">", "]");
		var formattedTarget = target.Replace("<", "[").Replace(">", "]");
		var formattedArrow = isResponse ? "-->> -" : "->> +";
		if (true)
		{
			Console.WriteLine();
		}
		return $"{formattedSource} {formattedArrow}{formattedTarget}: {invocationComment}";
	}
}