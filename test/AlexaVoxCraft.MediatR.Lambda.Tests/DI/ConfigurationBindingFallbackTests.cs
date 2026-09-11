using AlexaVoxCraft.MediatR.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.DI;

/// <summary>
/// Behavioral coverage for Issue #191 (docs/plans/0004-native-aot-runtime-fixes-and-validation.md,
/// Task Group 2): the non-generated fallback path for AddSkillMediator
/// (AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs), reached when
/// AlexaVoxCraft.MediatR.Generators never runs as an analyzer against the calling compilation - the
/// case for this project, which references AlexaVoxCraft.MediatR.Lambda without also referencing the
/// generator as an analyzer, per the "source-tree ProjectReference does not pull in the interceptor"
/// note in CLAUDE.md/ADR-0001. This proves the fallback path now binds
/// SkillServiceConfiguration via AlexaVoxCraft.MediatR.csproj's EnableConfigurationBindingGenerator
/// (Microsoft's Configuration Binding source generator) with identical behavior to the previous
/// reflection-based ConfigurationBinder.Bind, complementing AlexaVoxCraft.MediatR.Tests'
/// ConfigurationBindingTests, which covers the generated interceptor path instead.
/// </summary>
public sealed class ConfigurationBindingFallbackTests
{
    [Fact]
    public void AddSkillMediator_WithConfiguration_BindsEveryPropertyFromConfigurationSection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SkillConfiguration:CustomUserAgent"] = "MySkill/1.0",
                ["SkillConfiguration:SkillId"] = "amzn1.ask.skill.example",
                ["SkillConfiguration:DefaultVoiceName"] = "Matthew",
                ["SkillConfiguration:Lifetime"] = "Singleton",
                ["SkillConfiguration:CancellationTimeoutBufferMilliseconds"] = "500"
            })
            .Build();

        services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingFallbackTests>());

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value;

        options.CustomUserAgent.Should().Be("MySkill/1.0");
        options.SkillId.Should().Be("amzn1.ask.skill.example");
        options.DefaultVoiceName.Should().Be("Matthew");
        options.Lifetime.Should().Be(ServiceLifetime.Singleton);
        options.CancellationTimeoutBufferMilliseconds.Should().Be(500);
    }

    [Fact]
    public void AddSkillMediator_WithPartialConfiguration_LeavesMissingKeysAtTheirDeclaredDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SkillConfiguration:SkillId"] = "amzn1.ask.skill.partial"
            })
            .Build();

        services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingFallbackTests>());

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value;

        options.SkillId.Should().Be("amzn1.ask.skill.partial");
        options.CustomUserAgent.Should().BeNull();
        options.DefaultVoiceName.Should().BeNull();
        options.Lifetime.Should().Be(ServiceLifetime.Transient);
        options.CancellationTimeoutBufferMilliseconds.Should().Be(250);
    }
}
