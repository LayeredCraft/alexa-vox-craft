using System.Collections.Immutable;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request;
using Amazon.Lambda.Core;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlexaVoxCraft.MediatR.Generator.Tests;

/// <summary>
/// Utilities to compile one or more source files in-memory and run your incremental generator.
/// Uses <c>#if NET8_0/NET9_0/NET10_0</c> to select the correct BCL reference set.
/// </summary>
internal sealed class GeneratorTestHelpers
{
    private const string GlobalUsingsRelPath = "Cases/GlobalUsings.g.cs";
    
    /// <summary>
    /// Runs <paramref name="generator"/> against one or more C# files on disk.
    /// Files should live under <c>Cases/</c> and be copied to bin (see .csproj).
    /// </summary>
    public static (GeneratorDriver Driver, Compilation Original, CSharpParseOptions Parse)
        RunFromCases(IIncrementalGenerator generator,
            IEnumerable<string> caseRelativePaths,
            Dictionary<string, ReportDiagnostic>? diagnosticsToSuppress = null,
            Dictionary<string, string>? featureFlags = null,
            LanguageVersion languageVersion = LanguageVersion.Preview,
            IDictionary<string, string>? msbuildProperties = null)
    {
        var parse = CSharpParseOptions.Default
            .WithLanguageVersion(languageVersion)
            .WithFeatures(featureFlags);

        // Always include the single global-usings file first, if present
        var trees = new List<SyntaxTree>();

        var globalUsingsPath = ResolveCasePath(GlobalUsingsRelPath);
        if (File.Exists(globalUsingsPath))
        {
            var guText = File.ReadAllText(globalUsingsPath);
            trees.Add(CSharpSyntaxTree.ParseText(guText, parse, path: NormalizeForDiagnostics(globalUsingsPath)));
        }

        // Then include the case files passed in
        trees.AddRange(caseRelativePaths
            .Select(ResolveCasePath)
            .Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), parse, path: NormalizeForDiagnostics(p))));

        var references = GetBclReferences().ToList();
        references.AddRange([
            MetadataReference.CreateFromFile(typeof(AlexaSkillFunction<,>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SkillRequest).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IHostBuilder).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ServiceCollectionExtensions).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ILambdaContext).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IConfiguration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ConfigurationBinder).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(OptionsServiceCollectionExtensions).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ServiceLifetime).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(UserEventRequest).Assembly.Location),
        ]);

        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable
        );

        if (diagnosticsToSuppress is not null && diagnosticsToSuppress.Count > 0)
        {
            options = options.WithSpecificDiagnosticOptions(diagnosticsToSuppress.ToImmutableDictionary());
        }

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: trees,
            references: references,
            options: options
        );
        var obj = compilation.GetSpecialType(SpecialType.System_Object);
        if (obj.TypeKind == TypeKind.Error)
        {
            var refList = references
                .OfType<PortableExecutableReference>()
                .Select(r => System.IO.Path.GetFileName(r.FilePath ?? r.Display))
                .OrderBy(n => n)
                .ToArray();

            throw new InvalidOperationException(
                "BCL references missing: System.Object not resolved.\n" +
                "References seen:\n  - " + string.Join("\n  - ", refList)
            );
        }

        // ✅ Wire the analyzer config options provider (this is what you were after)
        AnalyzerConfigOptionsProvider? optionsProvider = null;
        if (msbuildProperties is { Count: > 0 })
        {
            // Expect FULL keys like "build_property.EnableMediatRGeneratorInterceptor"
            optionsProvider = new InMemoryAnalyzerConfigOptionsProvider(msbuildProperties);
        }        
        var classic = generator.AsSourceGenerator();
        var driver  = CSharpGeneratorDriver.Create(generators: [classic], optionsProvider: optionsProvider).RunGenerators(compilation);
        return (driver, compilation, parse);
    }

    /// <summary>
    /// Re-parses generated trees using the original parse options and returns a post-gen compilation for error checks.
    /// </summary>
    public static Compilation RecompileWithGeneratedTrees(Compilation original, CSharpParseOptions parse, GeneratorDriverRunResult runResult)
    {
        var reparsed = runResult.GeneratedTrees
            .Select(t => CSharpSyntaxTree.ParseText(t.GetText(), parse, path: t.FilePath ?? "Generated.g.cs"))
            .ToArray();

        return original.AddSyntaxTrees(reparsed);
    }

    /// <summary>
    /// Chooses the deterministic BCL reference set based on the current TFM via compiler symbols.
    /// </summary>
    private static IEnumerable<MetadataReference> GetBclReferences()
    {
#if NET8_0
        return Net80.References.All;
#elif NET9_0
        return Net90.References.All;
#elif NET10_0
        return Net100.References.All;
#else
        // Fallback so the file still compiles if a new TFM is added.
        return Net90.References.All;
#endif
    }

    /// <summary>Attempts bin-first resolution; falls back to relative path.</summary>
    private static string ResolveCasePath(string relative)
    {
        var candidate = Path.Combine(AppContext.BaseDirectory, relative);
        return File.Exists(candidate) ? candidate : relative;
    }

    private static string NormalizeForDiagnostics(string path) =>
        path.Replace(Path.DirectorySeparatorChar, '/');
}

/// <summary>
/// Verify glue that snapshots generator output and ensures the generated code compiles without errors.
/// </summary>
internal sealed class VerifyGlue
{
    /// <summary>
    /// Runs <paramref name="generator"/> against one or more input files, checks that generated code compiles,
    /// and snapshots the driver/run result to <c>Snapshots/</c>.
    /// </summary>
    public static async Task VerifySourcesAsync(IIncrementalGenerator generator,
        IEnumerable<string> casePaths,
        Dictionary<string, string>? featureFlags = null,
        Dictionary<string, ReportDiagnostic>? diagnosticsToSuppress = null,
        LanguageVersion languageVersion = LanguageVersion.Preview,
        IDictionary<string, string>? msbuildProperties = null)
    {
        var (driver, original, parse) = GeneratorTestHelpers.RunFromCases(
            generator,
            casePaths,
            diagnosticsToSuppress,
            featureFlags,
            languageVersion,
            msbuildProperties
        );

        driver.Should().NotBeNull();

        var result   = driver.GetRunResult();
        result.Should().NotBeNull();

        var compiled = GeneratorTestHelpers.RecompileWithGeneratedTrees(original, parse, result);
        var errors   = compiled.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();

        errors.Should().BeEmpty(
            "generated code should compile without errors, but found:\n" +
            string.Join("\n", errors.Select(e => $"  - {e.Id}: {e.GetMessage()} at {e.Location}"))
        );

        await Verifier
            .Verify(driver)
            .UseDirectory("Snapshots")
            .DisableDiff();
    }
}

/// <summary>
/// In-memory implementation of <see cref="AnalyzerConfigOptions"/> backed by a dictionary.
/// Keys are compared case-insensitively, e.g. "build_property.EnableMediatRGeneratorInterceptor".
/// </summary>
internal sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly Dictionary<string, string> _values;

    public InMemoryAnalyzerConfigOptions(IDictionary<string, string> values) =>
        _values = new(values, System.StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool TryGetValue(string key, out string value) =>
        _values.TryGetValue(key, out value!);
}

/// <summary>
/// In-memory <see cref="AnalyzerConfigOptionsProvider"/> that exposes
/// a global set of analyzer config options and returns empty options for per-file lookups.
/// </summary>
internal sealed class InMemoryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private static readonly AnalyzerConfigOptions Empty =
        new InMemoryAnalyzerConfigOptions(new Dictionary<string, string>());

    /// <summary>Creates a provider with the supplied global options (e.g., build_property.*).</summary>
    public InMemoryAnalyzerConfigOptionsProvider(IDictionary<string, string> globalOptions) =>
        GlobalOptions = new InMemoryAnalyzerConfigOptions(globalOptions);

    /// <inheritdoc />
    public override AnalyzerConfigOptions GlobalOptions { get; }

    /// <inheritdoc />
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => Empty;

    /// <inheritdoc />
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => Empty;
}
