using WatcherEye.Core.Documentation.Statements;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public interface IStatementCollector
{
	IReadOnlyCollection<ISequenceStatement> Statements { get; }
	void FillStackStatements();
	void PushStatement(ISequenceStatement statement);
	void AddStatement(ISequenceStatement statement);
}