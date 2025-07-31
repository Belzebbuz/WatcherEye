using Microsoft.CodeAnalysis;
using WatcherEye.Core.Documentation.Abstractions;
using WatcherEye.Core.Documentation.Dependencies;
using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Documentation.Collectors.Recursive;

public interface IDependencyCache
{
	ClassDependency GetOrAdd(INamedTypeSymbol symbol, ClassDependency dependency);

	IDependency AddOrUpdate(INamedTypeSymbol symbol,
		Func<INamedTypeSymbol, IDependency> addValueFactory,
		Func<INamedTypeSymbol, IDependency, IDependency> updateValueFactory);

	IReadOnlyCollection<IDependency> Dependencies { get;  }
	MethodDocumentation GetOrAdd(string key, MethodDocumentation documentation);
}