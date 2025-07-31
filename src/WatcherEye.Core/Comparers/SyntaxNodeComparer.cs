using Microsoft.CodeAnalysis;
using WatcherEye.Core.Utilities;

namespace WatcherEye.Core.Comparers;

public class SyntaxNodeComparer : IEqualityComparer<SyntaxNode>
{
	public static readonly SyntaxNodeComparer Instance = new SyntaxNodeComparer();
	public bool Equals(SyntaxNode? x, SyntaxNode? y)
	{
		if (x == null)
		{
			return y == null;
		}
		else if (y == null)
		{
			return false;
		}
		if(y == x)
			return true;
		
		return x.IsEquivalentTo(y) && x.Span.GetHashCode()== y.Span.GetHashCode();
	}

	public int GetHashCode(SyntaxNode obj)
	{
		var text = obj.GetText();
		var checksum = text.GetChecksum();
		var contentsHash = !checksum.IsDefault ? Hash.CombineValues(checksum) : 0;
		var encodingHash = text.Encoding != null ? text.Encoding.GetHashCode() : 0;
		return Hash.Combine(obj.Span.GetHashCode(), 
			Hash.Combine(contentsHash, 
				Hash.Combine(encodingHash, text.ChecksumAlgorithm.GetHashCode())));
	}
}