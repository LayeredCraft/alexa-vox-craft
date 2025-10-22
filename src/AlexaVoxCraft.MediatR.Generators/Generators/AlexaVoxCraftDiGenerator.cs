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
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node))
            .Where(static s => s is not null)
            .Select(static (s, _) => s!);

        var combined = callSiteProvider
            .Collect()
            .Combine(allTypes.Collect())
            .Combine(settings);

        context.RegisterSourceOutput(combined, static (spc, tuple) =>
        {
            var ((callSites, symbols), (interceptionEnabled, csharpOk)) = tuple;

            if (!interceptionEnabled || !csharpOk)
                return;

            if (callSites.Length == 0)
                return;

            var (model, discoveryDiagnostics) = SymbolDiscovery.BuildModel(symbols);
            foreach (var diagnostic in discoveryDiagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }

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

}

internal readonly struct InterceptorLocation
{
    public InterceptorLocation(string data)
    {
        Data = data;
    }

    public string Data { get; }
}