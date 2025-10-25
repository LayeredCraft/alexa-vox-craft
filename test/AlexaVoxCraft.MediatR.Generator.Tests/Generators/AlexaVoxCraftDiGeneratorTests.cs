using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Generators.Generators;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using Amazon.Lambda.Core;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlexaVoxCraft.MediatR.Generator.Tests.Generators;

/// <summary>
/// Tests for the AlexaVoxCraft dependency injection source generator.
/// Verifies that the generator correctly produces C# interceptors for AddSkillMediator calls.
/// </summary>
public class AlexaVoxCraftDiGeneratorTests
{
    /// <summary>
    /// Verifies that the generator produces an interceptor for a single AddSkillMediator call.
    /// The generated interceptor should have one InterceptsLocation attribute.
    /// </summary>
    [Fact]
    public async Task GeneratesInterceptor_ForSingleAddSkillMediatorCall()
    {
        var generator = new AlexaVoxCraftDiGenerator();
        await VerifyGlue.VerifySourcesAsync(generator,
            [
                "Cases/001_Simple/Function.cs"
            ],
            [
                MetadataReference.CreateFromFile(typeof(AlexaSkillFunction<,>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SkillRequest).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IHostBuilder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ServiceCollectionExtensions).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ILambdaContext).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IConfiguration).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ConfigurationBinder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OptionsServiceCollectionExtensions).Assembly.Location)
            ],
            new()
            {
                ["InterceptorsPreviewNamespaces"] = "AlexaVoxCraft.Generated",
                ["InterceptorsNamespaces"] = "AlexaVoxCraft.Generated",
            },
            msbuildProperties: new Dictionary<string, string>
            {
                ["build_property.EnableMediatRGeneratorInterceptor"] = "true"
            }
        );
    }
    
    /// <summary>
    /// Verifies that the generator produces a single interceptor method with multiple InterceptsLocation attributes
    /// when AddSkillMediator is called multiple times, demonstrating proper deduplication.
    /// </summary>
    [Fact]
    public async Task GeneratesInterceptor_ForMultipleAddSkillMediatorCalls_WithMultipleInterceptsLocations()
    {
        var generator = new AlexaVoxCraftDiGenerator();
        await VerifyGlue.VerifySourcesAsync(generator,
            [
                "Cases/002_MultiRegister/Function.cs"
            ],
            [
                MetadataReference.CreateFromFile(typeof(AlexaSkillFunction<,>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SkillRequest).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IHostBuilder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ServiceCollectionExtensions).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ILambdaContext).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IConfiguration).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ConfigurationBinder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OptionsServiceCollectionExtensions).Assembly.Location)
            ],
            new()
            {
                ["InterceptorsPreviewNamespaces"] = "AlexaVoxCraft.Generated",
                ["InterceptorsNamespaces"] = "AlexaVoxCraft.Generated",
            },
            msbuildProperties: new Dictionary<string, string>
            {
                ["build_property.EnableMediatRGeneratorInterceptor"] = "true"
            }
        );
    }
}