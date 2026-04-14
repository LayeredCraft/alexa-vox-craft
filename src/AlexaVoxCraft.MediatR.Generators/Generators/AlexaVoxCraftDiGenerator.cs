using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AlexaVoxCraft.MediatR.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

[Generator(LanguageNames.CSharp)]
public class AlexaVoxCraftDiGenerator : IIncrementalGenerator
{
    private const string TargetMethodName = "AddSkillMediator";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var interceptionEnabledSetting = context.AnalyzerConfigOptionsProvider
            .Select((x, _) =>
                x.GlobalOptions.TryGetValue($"build_property.{Constants.EnabledPropertyName}", out var enableSwitch)
                && !enableSwitch.Equals("false", StringComparison.Ordinal));

        var csharpSufficient = context.CompilationProvider
            .Select((x, _) => x is CSharpCompilation { LanguageVersion: LanguageVersion.Default or >= LanguageVersion.CSharp11 });

        var settings = interceptionEnabledSetting
            .Combine(csharpSufficient);

        var callSiteProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsAddSkillMediatorInvocation(node),
                static (ctx, _) => GetCallSiteData(ctx))
            .Where(static callSite => callSite.HasValue)
            .Select(static (callSite, _) => callSite!.Value);

        var explicitAssemblyNamesProvider = callSiteProvider
            .Collect()
            .Select(static (callSites, _) =>
            {
                var assemblyNames = callSites
                    .SelectMany(x => x.ExplicitAssemblyNames)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToArray();

                return new EquatableArray<string>(assemblyNames);
            });

        var allTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax,
                static (ctx, _) => ExtractTypeInfo(ctx))
            .Where(static t => t.HasValue)
            .Select(static (t, _) => t!.Value);

        var referencedAssemblyTypes = context.CompilationProvider
            .Combine(explicitAssemblyNamesProvider)
            .Select(static (data, _) => DiscoverReferencedAssemblyTypes(data.Left, data.Right));

        var modelWithDiagnostics = allTypes
            .Collect()
            .Combine(referencedAssemblyTypes)
            .Select(static (data, _) => BuildRegistrationModel(data.Left, data.Right));

        var combined = callSiteProvider
            .Collect()
            .Combine(modelWithDiagnostics)
            .Combine(settings);

        context.RegisterSourceOutput(combined, static (spc, tuple) =>
        {
            var ((callSites, modelData), (interceptionEnabled, csharpOk)) = tuple;

            if (!interceptionEnabled || !csharpOk)
                return;

            if (callSites.Length == 0)
                return;

            foreach (var diagnosticInfo in modelData.Diagnostics)
            {
                spc.ReportDiagnostic(Diagnostic.Create(diagnosticInfo.Descriptor, diagnosticInfo.Location));
            }

            var interceptorSource = InterceptorEmitter.EmitInterceptors(
                callSites.Select(c => c.Location).ToImmutableArray(),
                modelData.Model);
            spc.AddSource("__AlexaVoxCraft_Interceptors.g.cs",
                SourceText.From(interceptorSource, System.Text.Encoding.UTF8));
        });
    }

    private static bool IsAddSkillMediatorInvocation(SyntaxNode node) =>
        node is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.ValueText: TargetMethodName
            }
        };

    private static AddSkillMediatorCallSite? GetCallSiteData(GeneratorSyntaxContext ctx)
    {
        var invocation = (InvocationExpressionSyntax)ctx.Node;

        var op = ctx.SemanticModel.GetOperation(invocation) as IInvocationOperation;
        if (op is null) return null;

        var method = op.TargetMethod;
        if (method.Name != TargetMethodName) return null;

        // Map extension call to the underlying static method for identity checks
        var normalized = method.MethodKind == MethodKind.ReducedExtension
            ? method.ReducedFrom ?? method
            : method;

        var isOurType = normalized.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                        == "global::AlexaVoxCraft.MediatR.DI.ServiceCollectionExtensions";

        if (!isOurType) return null;

        // Only intercept the public method with IConfiguration parameter
        if (normalized.Parameters.Length < 2 ||
            normalized.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != "global::Microsoft.Extensions.Configuration.IConfiguration")
            return null;

        // Hash-based interceptable location
#pragma warning disable RSEXPERIMENTAL002
        var il = ctx.SemanticModel.GetInterceptableLocation(invocation, default);
#pragma warning restore RSEXPERIMENTAL002
        // Treat default/empty as "unavailable"
        if (il is null)
            return null;

        var explicitAssemblyNames = GetExplicitAssemblyNames(ctx, invocation, op);

        return new AddSkillMediatorCallSite(new InterceptorLocation(il.Data), explicitAssemblyNames);
    }

    private static EquatableArray<string> GetExplicitAssemblyNames(
        GeneratorSyntaxContext ctx,
        InvocationExpressionSyntax invocation,
        IInvocationOperation invocationOperation)
    {
        var settingsActionExpression = ResolveSettingsActionExpression(invocation, invocationOperation);

        if (settingsActionExpression is null)
        {
            return new EquatableArray<string>(Array.Empty<string>());
        }

        LambdaExpressionSyntax? lambda = settingsActionExpression switch
        {
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized,
            SimpleLambdaExpressionSyntax simple => simple,
            _ => null
        };

        if (lambda is null)
        {
            return new EquatableArray<string>(Array.Empty<string>());
        }

        var assemblyNames = new HashSet<string>(StringComparer.Ordinal);
        var registerCalls = lambda.Body
            .DescendantNodesAndSelf()
            .OfType<InvocationExpressionSyntax>()
            .Where(static call => call.Expression is MemberAccessExpressionSyntax
            {
                Name.Identifier.ValueText: "RegisterServicesFromAssemblyContaining"
            });

        foreach (var registerCall in registerCalls)
        {
            if (registerCall.Expression is not MemberAccessExpressionSyntax { Name: var memberName })
            {
                continue;
            }

            INamedTypeSymbol? typeSymbol = memberName switch
            {
                GenericNameSyntax { TypeArgumentList.Arguments.Count: 1 } genericName
                    => ctx.SemanticModel.GetTypeInfo(genericName.TypeArgumentList.Arguments[0]).Type as INamedTypeSymbol,
                IdentifierNameSyntax when registerCall.ArgumentList.Arguments.Count == 1
                    && registerCall.ArgumentList.Arguments[0].Expression is TypeOfExpressionSyntax typeOfExpression
                    => ctx.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol,
                _ => null
            };

            var assemblyName = typeSymbol?.ContainingAssembly?.Identity.Name;
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                assemblyNames.Add(assemblyName!);
            }
        }

        return new EquatableArray<string>(assemblyNames.OrderBy(x => x, StringComparer.Ordinal).ToArray());
    }

    private static ExpressionSyntax? ResolveSettingsActionExpression(
        InvocationExpressionSyntax invocation,
        IInvocationOperation invocationOperation)
    {
        var semanticMatch = invocationOperation.Arguments
            .FirstOrDefault(a => a.Parameter?.Name == "settingsAction");

        if (semanticMatch is not null && semanticMatch.Parameter is not null)
        {
            var semanticValue = semanticMatch.Value;
            if (semanticValue is not null && semanticValue.Syntax is ExpressionSyntax semanticExpression)
            {
                return semanticExpression;
            }
        }

        var namedArgument = invocation.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameColon?.Name.Identifier.ValueText == "settingsAction");
        if (namedArgument is not null)
        {
            return namedArgument.Expression;
        }

        if (invocation.ArgumentList.Arguments.Count >= 2)
        {
            return invocation.ArgumentList.Arguments[1].Expression;
        }

        return null;
    }

    private static ModelWithDiagnostics BuildRegistrationModel(
        ImmutableArray<DiscoveredTypeInfo> sourceTypes,
        EquatableArray<DiscoveredTypeInfo> referencedAssemblyTypes)
    {
        var merged = new List<DiscoveredTypeInfo>(sourceTypes.Length + referencedAssemblyTypes.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var typeInfo in sourceTypes)
        {
            if (seen.Add(typeInfo.FullyQualifiedTypeName))
            {
                merged.Add(typeInfo);
            }
        }

        foreach (var typeInfo in referencedAssemblyTypes)
        {
            if (seen.Add(typeInfo.FullyQualifiedTypeName))
            {
                merged.Add(typeInfo);
            }
        }

        return SymbolDiscovery.BuildModel(merged.ToImmutableArray());
    }

    private static EquatableArray<DiscoveredTypeInfo> DiscoverReferencedAssemblyTypes(
        Compilation compilation,
        EquatableArray<string> explicitAssemblyNames)
    {
        if (explicitAssemblyNames.Count == 0)
        {
            return new EquatableArray<DiscoveredTypeInfo>(Array.Empty<DiscoveredTypeInfo>());
        }

        var requestedAssemblyNames = new HashSet<string>(explicitAssemblyNames, StringComparer.Ordinal);
        var discoveredTypes = new List<DiscoveredTypeInfo>();

        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            if (!requestedAssemblyNames.Contains(assembly.Identity.Name))
            {
                continue;
            }

            DiscoverTypesFromNamespace(assembly.GlobalNamespace, discoveredTypes);
        }

        return new EquatableArray<DiscoveredTypeInfo>(discoveredTypes);
    }

    private static void DiscoverTypesFromNamespace(INamespaceSymbol namespaceSymbol, List<DiscoveredTypeInfo> discoveredTypes)
    {
        foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
        {
            DiscoverTypeAndNestedTypes(typeSymbol, discoveredTypes);
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            DiscoverTypesFromNamespace(childNamespace, discoveredTypes);
        }
    }

    private static void DiscoverTypeAndNestedTypes(INamedTypeSymbol typeSymbol, List<DiscoveredTypeInfo> discoveredTypes)
    {
        var typeInfo = ExtractTypeInfo(typeSymbol);
        if (typeInfo.HasValue)
        {
            discoveredTypes.Add(typeInfo.Value);
        }

        foreach (var nestedType in typeSymbol.GetTypeMembers())
        {
            DiscoverTypeAndNestedTypes(nestedType, discoveredTypes);
        }
    }

    private static DiscoveredTypeInfo? ExtractTypeInfo(GeneratorSyntaxContext ctx)
    {
        var symbol = ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node) as INamedTypeSymbol;
        return symbol is null ? null : ExtractTypeInfo(symbol);
    }

    private static DiscoveredTypeInfo? ExtractTypeInfo(INamedTypeSymbol symbol)
    {
        const string AlexaHandlerAttributeName = "AlexaVoxCraft.MediatR.Annotations.AlexaHandlerAttribute";

        var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var location = symbol.Locations.FirstOrDefault() ?? Location.None;

        var attribute = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AlexaHandlerAttributeName);

        AttributeInfo? attributeInfo = null;
        if (attribute != null)
        {
            var lifetime = 2;
            var order = 0;
            var exclude = false;

            var lifetimeArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Lifetime");
            if (lifetimeArg.Value.Value is int lifetimeValue)
            {
                lifetime = lifetimeValue;
            }

            var orderArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Order");
            if (orderArg.Value.Value is int orderValue)
            {
                order = orderValue;
            }

            var excludeArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Exclude");
            if (excludeArg.Value.Value is bool excludeValue)
            {
                exclude = excludeValue;
            }

            attributeInfo = new AttributeInfo(lifetime, order, exclude);
        }

        var interfaces = new List<string>();
        foreach (var iface in symbol.AllInterfaces)
        {
            interfaces.Add(iface.ToDisplayString());
        }

        return new DiscoveredTypeInfo(
            fullyQualifiedName,
            symbol.IsAbstract,
            symbol.IsGenericType && symbol.IsUnboundGenericType,
            symbol.TypeKind,
            attributeInfo,
            new EquatableArray<string>(interfaces),
            location
        );
    }

}

internal readonly record struct InterceptorLocation(string Data);

internal readonly record struct AddSkillMediatorCallSite(
    InterceptorLocation Location,
    EquatableArray<string> ExplicitAssemblyNames
);
