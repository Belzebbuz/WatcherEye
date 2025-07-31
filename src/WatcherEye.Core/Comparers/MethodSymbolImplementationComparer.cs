using Microsoft.CodeAnalysis;

namespace WatcherEye.Core.Comparers;

public class MethodSymbolImplementationComparer : IEqualityComparer<IMethodSymbol>
{
	public static readonly MethodSymbolImplementationComparer Instance = new MethodSymbolImplementationComparer();
	private static readonly SymbolDisplayFormat Format = new SymbolDisplayFormat(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions:
		SymbolDisplayGenericsOptions.IncludeTypeParameters |
		SymbolDisplayGenericsOptions.IncludeVariance |
		SymbolDisplayGenericsOptions.IncludeTypeConstraints,
		memberOptions:
		SymbolDisplayMemberOptions.IncludeParameters |
		SymbolDisplayMemberOptions.IncludeExplicitInterface,
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
	public bool Equals(IMethodSymbol? x, IMethodSymbol? y)
	{
		if (x == null)
			return y == null;
		else if (y == null)
			return false;
		var xSignature = $"{x.ReturnType.ToDisplayString(Format)} : {x.ToDisplayString(Format)}";
		var ySignature = $"{y.ReturnType.ToDisplayString(Format)} : {y.ToDisplayString(Format)}";
		return xSignature == ySignature;
	}

	public int GetHashCode(IMethodSymbol obj)
	{
		return $"{obj.ReturnType.ToDisplayString(Format)} : {obj.ToDisplayString(Format)}".GetHashCode();
	}
}