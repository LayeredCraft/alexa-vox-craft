using System.Collections.Immutable;
using AlexaVoxCraft.MediatR.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

[Generator(LanguageNames.CSharp)]
public class AlexaVoxCraftDiGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Collect all declared type symbols
        var allTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax,
                static (ctx, _) =>
                    ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node) as INamedTypeSymbol)
            .Where(static s => s is not null)!;

        // Build the registration model using SymbolDiscovery
        var model = allTypes
            .Collect()
            .Select(static (symbols, _) => SymbolDiscovery.BuildModel(symbols));

        // Emit generated source code using RegistrationEmitter
        context.RegisterSourceOutput(model, static (spc, m) =>
        {
            var source = RegistrationEmitter.EmitServiceRegistrations(m);
            var src = SourceText.From(source, System.Text.Encoding.UTF8);
            spc.AddSource("__AlexaVoxCraft_Generated.g.cs", src);
        });
    }
}