namespace WatcherEye.Core.Documentation.Statements;

public class OpenCallLocalMethodStatement : ISequenceStatement
{
	public required string Name { get; init; }
	public CloseCallLocalMethodStatement? CloseStatement { get; set; }
}