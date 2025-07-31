namespace WatcherEye.Core.Documentation.Statements;

public class OpenIfStatement : ISequenceStatement
{
	public required string Condition { get; init; }
	public CloseIfStatement? CloseStatement { get; set; }
}