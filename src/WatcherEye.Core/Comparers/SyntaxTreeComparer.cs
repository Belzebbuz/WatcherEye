using Microsoft.CodeAnalysis;
using WatcherEye.Core.Utilities;

namespace WatcherEye.Core.Comparers;

public class SyntaxTreeComparer : IEqualityComparer<SyntaxTree>
{
	public static readonly SyntaxTreeComparer Instance = new SyntaxTreeComparer();
	public bool Equals(SyntaxTree? x, SyntaxTree? y)
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
		
		return x.GetRoot().IsEquivalentTo(y.GetRoot()) &&
		       string.Equals(x.FilePath, y.FilePath, StringComparison.OrdinalIgnoreCase);
	}

	public int GetHashCode(SyntaxTree obj)
	{
		var text = obj.GetText();
		var checksum = text.GetChecksum();
		var contentsHash = !checksum.IsDefault ? Hash.CombineValues(checksum) : 0;
		var encodingHash = text.Encoding != null ? text.Encoding.GetHashCode() : 0;
		var textHashCode = Hash.Combine(contentsHash,
			Hash.Combine(encodingHash, text.ChecksumAlgorithm.GetHashCode()));
		return Hash.Combine(obj.FilePath.GetHashCode(), textHashCode);
	}
}