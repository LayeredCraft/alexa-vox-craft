using System;
using System.Collections.Immutable;
using System.Linq;
using AlexaVoxCraft.MediatR.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

[Generator(LanguageNames.CSharp)]
public class AlexaVoxCraftDiGenerator : IIncrementalGenerator
{
    private const string AttributeName = "AlexaVoxCraft.MediatR.Annotations.AlexaVoxCraftRegistrationAttribute";
    private const string PartialClassName = "AlexaVoxCraftRegistration";
    private const string PartialMethodName = "AddAlexaSkillMediator";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Check if generator is enabled (defaults to true if property not set)
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

        // Discover partial class candidates
        var partialClassProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsPartialClassCandidate(node),
                static (ctx, _) => GetPartialClassInfo(ctx))
            .Where(static info => info is not null);

        // Collect all declared type symbols for service discovery
        var allTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax,
                static (ctx, _) =>
                    ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node) as INamedTypeSymbol)
            .Where(static s => s is not null)!;

        // Combine everything
        var combined = partialClassProvider
            .Collect()
            .Combine(allTypes.Collect())
            .Combine(enabledProvider);

        // Emit generated source code
        context.RegisterSourceOutput(combined, static (spc, tuple) =>
        {
            var ((partials, symbols), enabled) = tuple;

            if (!enabled)
                return;

            // Find the partial class to implement
            var partialInfo = partials.FirstOrDefault();
            if (partialInfo is null)
                return;

            // Build the registration model
            var model = SymbolDiscovery.BuildModel(symbols);

            // Emit the partial implementation
            var source = RegistrationEmitter.EmitPartialMethod(partialInfo.Value, model);
            var src = SourceText.From(source, System.Text.Encoding.UTF8);
            spc.AddSource($"{partialInfo.Value.ClassName}.g.cs", src);
        });
    }

    private static bool IsPartialClassCandidate(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        return classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
            && classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))
            && classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
    }

    private static PartialClassInfo? GetPartialClassInfo(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(classDecl);

        if (symbol is null)
            return null;

        // Check for attribute or naming convention
        bool hasAttribute = symbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == AttributeName);

        bool matchesConvention = symbol.Name == PartialClassName;

        if (!hasAttribute && !matchesConvention)
            return null;

        // Verify the partial method exists with correct signature
        // Should have: IServiceCollection, IConfiguration, Action<SkillServiceConfiguration>?, string
        var method = symbol.GetMembers(PartialMethodName)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.IsPartialDefinition
                              && m.IsStatic
                              && m.IsExtensionMethod
                              && m.Parameters.Length >= 2
                              && m.Parameters[0].Type.ToDisplayString() == "Microsoft.Extensions.DependencyInjection.IServiceCollection"
                              && m.Parameters[1].Type.ToDisplayString() == "Microsoft.Extensions.Configuration.IConfiguration");

        if (method is null)
            return null;

        return new PartialClassInfo(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            PartialMethodName
        );
    }
}

internal readonly struct PartialClassInfo
{
    public PartialClassInfo(string namespaceName, string className, string methodName)
    {
        Namespace = namespaceName;
        ClassName = className;
        MethodName = methodName;
    }

    public string Namespace { get; }
    public string ClassName { get; }
    public string MethodName { get; }
}