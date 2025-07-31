using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using WatcherEye.Core.Comparers;

namespace WatcherEye.Core.Implementations;

public static class WatcherSymbolCache<TValue> 
{
	private static readonly ConcurrentDictionary<ISymbol, TValue> Cache = new(SymbolByNameEqualityComparer.Instance);
	public static TValue GetOrCreate(ISymbol key, TValue value) => Cache.GetOrAdd(key, value);
	public static TValue GetOrCreate(ISymbol key, Func<ISymbol,TValue> valueFactory) => Cache.GetOrAdd(key, valueFactory);
}

public static class WatcherSyntaxNodeCache<TValue>
{
	private static readonly ConcurrentDictionary<SyntaxNode, TValue> Cache = new(SyntaxNodeComparer.Instance);
	public static TValue GetOrCreate(SyntaxNode key, Func<SyntaxNode,TValue> valueFactory) => Cache.GetOrAdd(key, valueFactory);
}