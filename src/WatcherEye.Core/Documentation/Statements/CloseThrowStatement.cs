namespace WatcherEye.Core.Documentation.Statements;

public class CloseThrowStatement : ISequenceStatement
{
	public required OpenThrowStatement OpenStatement { get; init; }
}