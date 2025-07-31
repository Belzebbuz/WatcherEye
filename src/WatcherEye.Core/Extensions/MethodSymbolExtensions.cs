using Microsoft.CodeAnalysis;
using WatcherEye.Core.Documentation.Results;

namespace WatcherEye.Core.Extensions;

public static class MethodSymbolExtensions
{
	private static readonly SymbolDisplayFormat MethodOverloadFormat = new SymbolDisplayFormat(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
		genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
		memberOptions:
		SymbolDisplayMemberOptions.IncludeParameters,
		parameterOptions:
		SymbolDisplayParameterOptions.IncludeType |
		SymbolDisplayParameterOptions.IncludeParamsRefOut |
		SymbolDisplayParameterOptions.IncludeExtensionThis |
		SymbolDisplayParameterOptions.IncludeDefaultValue,
		miscellaneousOptions:
		SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
		SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

	private static readonly SymbolDisplayFormat FullSignatureName = new SymbolDisplayFormat(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions:
		SymbolDisplayGenericsOptions.IncludeTypeParameters |
		SymbolDisplayGenericsOptions.IncludeTypeConstraints,
		memberOptions:
		SymbolDisplayMemberOptions.IncludeType |
		SymbolDisplayMemberOptions.IncludeParameters |
		SymbolDisplayMemberOptions.IncludeModifiers |
		SymbolDisplayMemberOptions.IncludeAccessibility,
		parameterOptions:
		SymbolDisplayParameterOptions.IncludeType |
		SymbolDisplayParameterOptions.IncludeName |
		SymbolDisplayParameterOptions.IncludeParamsRefOut |
		SymbolDisplayParameterOptions.IncludeExtensionThis |
		SymbolDisplayParameterOptions.IncludeOptionalBrackets,
		miscellaneousOptions:
		SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
		SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
		SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
		SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral,
		kindOptions:
		SymbolDisplayKindOptions.IncludeTypeKeyword);

	public static IReadOnlyCollection<MethodParameter> GetParameters(this IMethodSymbol methodSymbol)
	{
		return methodSymbol.Parameters.Select(x =>
		{
			var declarationUrl = GetDeclarationUrl(x.Type);
			return new MethodParameter(x.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				x.Name,
				declarationUrl);
		}).ToArray().AsReadOnly();
	}

	public static string? GetDeclarationUrl(this ISymbol symbol)
	{
		var declarationSyntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
		var isGenerated = declarationSyntax?.SyntaxTree.IsGenerated();
		string? declarationUrl = null;
		if (declarationSyntax is not null && isGenerated is false)
			declarationUrl = declarationSyntax.GetPath();
		return declarationUrl;
	}

	public static IReadOnlyCollection<TypeParameter> GetTypeParameters(this IMethodSymbol methodSymbol)
	{
		return methodSymbol.TypeParameters.Select(x =>
		{
			var declarationUrl = GetDeclarationUrl(x);
			return new TypeParameter(x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				declarationUrl);
		}).ToArray().AsReadOnly();
		;
	}

	public static string GetOverloadName(this IMethodSymbol methodSymbol)
	{
		return methodSymbol.ToDisplayString(MethodOverloadFormat);
	}

	public static string GetFullSignatureName(this IMethodSymbol methodSymbol)
	{
		return methodSymbol.ToDisplayString(FullSignatureName);
	}
}

public static class NamedTypeSymbolExtensions
{
	public static IReadOnlyCollection<TypeParameter> GetTypeArguments(this INamedTypeSymbol? methodSymbol)
	{
		if (methodSymbol == null)
			return [];
		return methodSymbol.TypeArguments.Select(x =>
			new TypeParameter(x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				x.GetDeclarationUrl())).ToArray().AsReadOnly();
	}
}