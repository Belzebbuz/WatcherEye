namespace WatcherEye.Core.Documentation.Statements;

public class CloseElseStatement : ISequenceStatement
{
	public required OpenElseStatement OpenStatement { get; init; }
}