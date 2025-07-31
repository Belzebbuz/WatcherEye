namespace WatcherEye.Core.Documentation.Statements;

public class OpenElseStatement : ISequenceStatement
{
	public CloseElseStatement? CloseStatement { get; set; }
}