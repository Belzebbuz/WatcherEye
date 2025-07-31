namespace WatcherEye.Core.Documentation.Statements;

public class OpenObjectCreationStatement : ISequenceStatement
{
	public required string ObjectType { get; init; }
	public CloseObjectCreationStatement? CloseStatement { get; set; }
}