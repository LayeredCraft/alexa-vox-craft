using AlexaVoxCraft.MediatR.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Tests.Registration;

/// <summary>
/// Behavioral coverage for Issue #191 (docs/plans/0004-native-aot-runtime-fixes-and-validation.md,
/// Task Group 2): AddSkillMediator(IConfiguration, ...) no longer binds SkillServiceConfiguration via
/// the reflection-based ConfigurationBinder.Bind. These tests drive the actual generated
/// AddSkillMediator interceptor (AlexaVoxCraft.MediatR.Generators' InterceptorEmitter,
/// BindSkillServiceConfiguration) through a real IConfiguration and assert every bindable property,
/// including default/partial-configuration fallback - not merely that the emitted source compiles or
/// that the AOT/trim warning is gone. This project references AlexaVoxCraft.MediatR.Generators as an
/// analyzer (see the .csproj) specifically so the call sites below are intercepted for real, rather
/// than falling through to AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs's hand-written
/// fallback path (covered separately by ConfigurationBindingFallbackTests).
/// </summary>
public sealed class ConfigurationBindingTests
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
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>());

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
                // CustomUserAgent, DefaultVoiceName, Lifetime, CancellationTimeoutBufferMilliseconds
                // intentionally absent - must fall back to SkillServiceConfiguration's declared defaults.
            })
            .Build();

        services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>());

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value;

        options.SkillId.Should().Be("amzn1.ask.skill.partial");
        options.CustomUserAgent.Should().BeNull();
        options.DefaultVoiceName.Should().BeNull();
        options.Lifetime.Should().Be(ServiceLifetime.Transient);
        options.CancellationTimeoutBufferMilliseconds.Should().Be(250);
    }

    // Regression test for a review finding on this PR: a key that IS present but holds an empty/
    // malformed scalar value must not be treated the same as a genuinely missing key. The original
    // ConfigurationBinder.Bind threw for this (verified empirically: config.Bind(target) with
    // Lifetime="" throws InvalidOperationException) - the reflection-free interceptor replacement must
    // preserve that fail-loudly behavior, not silently fall back to SkillServiceConfiguration's default.
    [Fact]
    public void AddSkillMediator_WithEmptyLifetimeValue_ThrowsInsteadOfSilentlyKeepingDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SkillConfiguration:Lifetime"] = ""
            })
            .Build();

        var act = () => services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddSkillMediator_WithEmptyCancellationTimeoutBufferMillisecondsValue_ThrowsInsteadOfSilentlyKeepingDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SkillConfiguration:CancellationTimeoutBufferMilliseconds"] = ""
            })
            .Build();

        var act = () => services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddSkillMediator_WithSettingsActionAfterConfiguration_SettingsActionValuesWin()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SkillConfiguration:SkillId"] = "amzn1.ask.skill.from-configuration"
            })
            .Build();

        services.AddSkillMediator(configuration, cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>();
            cfg.SkillId = "amzn1.ask.skill.from-settings-action";
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value;

        options.SkillId.Should().Be("amzn1.ask.skill.from-settings-action");
    }

    [Fact]
    public void AddSkillMediator_WithCustomSectionName_BindsFromThatSection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomSection:SkillId"] = "amzn1.ask.skill.custom-section",
                ["CustomSection:Lifetime"] = "Scoped"
            })
            .Build();

        services.AddSkillMediator(configuration,
            cfg => cfg.RegisterServicesFromAssemblyContaining<ConfigurationBindingTests>(),
            "CustomSection");

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value;

        options.SkillId.Should().Be("amzn1.ask.skill.custom-section");
        options.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}
