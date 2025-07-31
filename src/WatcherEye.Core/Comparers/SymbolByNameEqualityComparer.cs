using Microsoft.CodeAnalysis;

namespace WatcherEye.Core.Comparers;

public class SymbolByNameEqualityComparer : IEqualityComparer<ISymbol>
{
	public static readonly SymbolByNameEqualityComparer Instance = new SymbolByNameEqualityComparer();
	private static readonly SymbolDisplayFormat Format = new SymbolDisplayFormat(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions:
		SymbolDisplayGenericsOptions.IncludeTypeParameters |
		SymbolDisplayGenericsOptions.IncludeVariance |
		SymbolDisplayGenericsOptions.IncludeTypeConstraints,
		memberOptions:
		SymbolDisplayMemberOptions.IncludeParameters |
		SymbolDisplayMemberOptions.IncludeExplicitInterface | SymbolDisplayMemberOptions.IncludeContainingType,
		parameterOptions:
		SymbolDisplayParameterOptions.IncludeType |
		SymbolDisplayParameterOptions.IncludeName |
		SymbolDisplayParameterOptions.IncludeParamsRefOut |
		SymbolDisplayParameterOptions.IncludeDefaultValue |
		SymbolDisplayParameterOptions.IncludeExtensionThis |
		SymbolDisplayParameterOptions.IncludeOptionalBrackets,
		propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor, 
		miscellaneousOptions:
		SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
		SymbolDisplayMiscellaneousOptions.UseSpecialTypes
	);
	public bool Equals(ISymbol? x, ISymbol? y)
	{
		if (x == null)
		{
			return y == null;
		}
		else if (y == null)
		{
			return false;
		}
		
		var xName = x.ToDisplayString(Format);
		var yName = y.ToDisplayString(Format);
		
		return string.Equals(xName, yName) && x.ContainingAssembly.Identity.Equals(y.ContainingAssembly.Identity);
	}

	public int GetHashCode(ISymbol obj)
	{
		return obj.ToDisplayString(Format).GetHashCode();
	}
}