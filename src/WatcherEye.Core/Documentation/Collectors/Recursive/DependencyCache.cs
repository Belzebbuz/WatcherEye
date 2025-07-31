using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using WatcherEye.Core.Comparers;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Dependencies;
using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public class DependencyCache : IDependencyCache
{
	private readonly ConcurrentDictionary<INamedTypeSymbol, ClassDependency> _dependencies =
		new(SymbolByNameEqualityComparer.Instance);

	private readonly ConcurrentDictionary<INamedTypeSymbol, IDependency> _allDependencies =
		new(SymbolByNameEqualityComparer.Instance);
	
	private readonly ConcurrentDictionary<string, MethodDocumentation> _methods = new();
	
	public IReadOnlyCollection<IDependency> Dependencies => _allDependencies.Values.ToArray().AsReadOnly();
	public MethodDocumentation GetOrAdd(string key, MethodDocumentation documentation)
	{
		 return _methods.GetOrAdd(key, documentation);
	}

	public ClassDependency GetOrAdd(
		INamedTypeSymbol symbol,
		ClassDependency dependency)
	{
		return _dependencies.GetOrAdd(symbol, dependency);
	}

	public IDependency AddOrUpdate(INamedTypeSymbol symbol,
		Func<INamedTypeSymbol, IDependency> addValueFactory,
		Func<INamedTypeSymbol, IDependency, IDependency> updateValueFactory)
	{
		return _allDependencies.AddOrUpdate(symbol, addValueFactory, updateValueFactory);
	}

}