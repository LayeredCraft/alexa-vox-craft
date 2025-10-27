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
            .Select((x,_) => x is CSharpCompilation { LanguageVersion: LanguageVersion.Default or >= LanguageVersion.CSharp11 });

        var settings = interceptionEnabledSetting
            .Combine(csharpSufficient);

        var callSiteProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsAddSkillMediatorInvocation(node),
                static (ctx, _) => GetCallSiteLocation(ctx))
            .Where(static loc => loc.HasValue)
            .Select(static (loc, _) => loc!.Value);

        var allTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax,
                static (ctx, _) => ExtractTypeInfo(ctx))
            .Where(static t => t.HasValue)
            .Select(static (t, _) => t!.Value);

        var modelWithDiagnostics = allTypes
            .Collect()
            .Select(static (types, _) => SymbolDiscovery.BuildModel(types));

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

            var interceptorSource = InterceptorEmitter.EmitInterceptors(callSites.ToImmutableArray(), modelData.Model);
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

    private static InterceptorLocation? GetCallSiteLocation(GeneratorSyntaxContext ctx)
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

        return new InterceptorLocation(il.Data);
    }

    private static DiscoveredTypeInfo? ExtractTypeInfo(GeneratorSyntaxContext ctx)
    {
        const string AlexaHandlerAttributeName = "AlexaVoxCraft.MediatR.Annotations.AlexaHandlerAttribute";

        var symbol = ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node) as INamedTypeSymbol;
        if (symbol is null)
            return null;

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