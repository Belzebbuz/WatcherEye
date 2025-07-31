namespace WatcherEye.Core.Documentation.Statements;

public class CloseIfStatement : ISequenceStatement
{
	public required OpenIfStatement OpenStatement { get; init; }
}