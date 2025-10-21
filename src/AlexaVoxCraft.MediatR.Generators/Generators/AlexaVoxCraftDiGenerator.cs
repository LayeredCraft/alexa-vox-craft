using System;
using System.Collections.Immutable;
using System.Linq;
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
        var enabledProvider = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) =>
            {
                if (provider.GlobalOptions.TryGetValue("build_property.alexavoxcraftgeneratorenabled", out var value))
                {
                    return !("false".Equals(value, StringComparison.OrdinalIgnoreCase)
                             || "0".Equals(value, StringComparison.OrdinalIgnoreCase)
                             || "no".Equals(value, StringComparison.OrdinalIgnoreCase));
                }

                return true;
            });

        var interceptorsValueProvider = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.interceptorsnamespaces", out var v1);
                provider.GlobalOptions.TryGetValue("build_property.interceptorspreviewnamespaces",
                    out var v2); // older name
                // Return both; we’ll check both later
                return (v1 ?? string.Empty, v2 ?? string.Empty);
            });

        var interceptorsEnabledProvider = interceptorsValueProvider
            .Select(static (vals, _) =>
            {
                var (v1, v2) = vals;

                static bool HasNs(string s) =>
                    s.Split(';', ',')
                        .Select(x => x.Trim())
                        .Any(x => x.Equals("AlexaVoxCraft.Generated", StringComparison.Ordinal));

                return HasNs(v1 ?? string.Empty) || HasNs(v2 ?? string.Empty);
            });

        var callSiteProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsAddSkillMediatorInvocation(node),
                static (ctx, _) => GetCallSiteLocation(ctx))
            .Where(static loc => loc.HasValue)
            .Select(static (loc, _) => loc!.Value);
        
        var allTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax,
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node))
            .Where(static s => s is not null)
            .Select(static (s, _) => s!);  // upcast + null-forgiving
        
        var combined = callSiteProvider
            .Collect()
            .Combine(allTypes.Collect())
            .Combine(enabledProvider)
            .Combine(interceptorsEnabledProvider)
            .Combine(interceptorsValueProvider);

        context.RegisterSourceOutput(combined, static (spc, tuple) =>
        {
            var ((((callSites, symbols), enabled), interceptorsEnabled), (interceptorsNs, interceptorsPreviewNs)) =
                tuple;

            if (!enabled)
                return; // hard off-switch

            if (!interceptorsEnabled)
            {
                var shown = string.IsNullOrWhiteSpace(interceptorsNs) ? interceptorsPreviewNs : interceptorsNs;
                spc.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InterceptorsDisabled, Location.None, shown ?? "<null>"));
                return;
            }

            if (callSites.Length == 0)
            {
                spc.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NoCallSites, Location.None));
                return;
            }

            // Build the model and report any diagnostics from symbol discovery
            var (model, discoveryDiagnostics) = SymbolDiscovery.BuildModel(symbols);
            foreach (var diagnostic in discoveryDiagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }

            // Emit the interceptor
            var interceptorSource = InterceptorEmitter.EmitInterceptors(callSites.ToImmutableArray(), model);
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

        // Hash-based interceptable location
#pragma warning disable RSEXPERIMENTAL002
        var il = ctx.SemanticModel.GetInterceptableLocation(invocation, default);
#pragma warning restore RSEXPERIMENTAL002
        // Treat default/empty as “unavailable”
        if (il is null)
            return null;

        return new InterceptorLocation(il.Data);
    }

}

internal readonly struct InterceptorLocation
{
    public InterceptorLocation(string data)
    {
        Data = data;
    }

    public string Data { get; }
}