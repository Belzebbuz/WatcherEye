namespace WatcherEye.Core.Documentation.Statements;

public class CloseObjectCreationStatement : ISequenceStatement
{
	public required OpenObjectCreationStatement OpenStatement { get; init; }
}