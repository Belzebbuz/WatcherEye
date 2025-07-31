using Microsoft.CodeAnalysis;
using WatcherEye.Core.Documentation.Statements;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class StatementCollector : IStatementCollector
{
	private readonly List<ISequenceStatement> _statements = new();
	public IReadOnlyCollection<ISequenceStatement> Statements => _statements.AsReadOnly();
	private readonly Stack<ISequenceStatement> _tempStatements = new();

	private readonly Dictionary<SyntaxNode, List<ISequenceStatement>> _statementsByNode = new();
	public void FillStackStatements()
	{
		while (_tempStatements.TryPop(out var dependency))
		{
			_statements?.Add(dependency);
		}

		_tempStatements.Clear();
	}

	public void PushStatement(ISequenceStatement statement) => _tempStatements.Push(statement);
	public void AddStatement(ISequenceStatement statement)
	{
		_statements.Add(statement);
	}
}