namespace WatcherEye.Core.Documentation.Statements;

public class CloseCallLocalMethodStatement : ISequenceStatement
{
	public required OpenCallLocalMethodStatement OpenStatement { get; init; }
}