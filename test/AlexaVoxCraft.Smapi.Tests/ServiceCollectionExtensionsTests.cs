using AlexaVoxCraft.Http;
using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.Smapi.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSmapiDeveloperClient_WithOptionsActionAndHttpClientCustomization_InvokesCustomizationCallback()
    {
        var services = new ServiceCollection();
        var callbackInvoked = false;

        services.AddSmapiDeveloperClient(
            options =>
            {
                options.ClientId = "client-id";
                options.ClientSecret = "client-secret";
                options.RefreshToken = "refresh-token";
            },
            _ => callbackInvoked = true);

        callbackInvoked.Should().BeTrue();
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithConfigurationAndHttpClientCustomization_InvokesCustomizationCallback()
    {
        var services = new ServiceCollection();
        var callbackInvoked = false;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmapiClient:ClientId"] = "client-id",
                ["SmapiClient:ClientSecret"] = "client-secret",
                ["SmapiClient:RefreshToken"] = "refresh-token"
            })
            .Build();

        services.AddSmapiDeveloperClient(configuration, configureHttpClientBuilder: _ => callbackInvoked = true);

        callbackInvoked.Should().BeTrue();
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithOptionsAction_RegistersRequiredServices()
    {
        var services = new ServiceCollection();

        services.AddSmapiDeveloperClient(options =>
        {
            options.ClientId = "client-id";
            options.ClientSecret = "client-secret";
            options.RefreshToken = "refresh-token";
        });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IAlexaInteractionModelClient>().Should().NotBeNull();
        provider.GetService<IAccessTokenProvider>().Should().NotBeNull();
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithNullConfiguration_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        var act = () => services.AddSmapiDeveloperClient((IConfiguration)null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithNullOptionsAction_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        var act = () => services.AddSmapiDeveloperClient((Action<SmapiDeveloperAccessTokenOptions>)null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("optionsAction");
    }

    // Behavioral coverage for Issue #191 (docs/plans/0004-native-aot-runtime-fixes-and-validation.md,
    // Task Group 2): AddSmapiDeveloperClient(IConfiguration, ...) binds SmapiDeveloperAccessTokenOptions
    // via the Microsoft.Extensions.Configuration.Binder source generator now (AlexaVoxCraft.Smapi.csproj's
    // EnableConfigurationBindingGenerator) instead of the reflection-based ConfigurationBinder.Bind. This
    // proves binding semantics, property by property, survive that change - not just that the AOT/trim
    // warning is gone.
    [Fact]
    public void AddSmapiDeveloperClient_WithConfiguration_BindsEveryPropertyFromConfigurationSection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmapiClient:ClientId"] = "amzn1.application-oa2-client.example",
                ["SmapiClient:ClientSecret"] = "example-secret",
                ["SmapiClient:RefreshToken"] = "Atzr|example-refresh-token"
            })
            .Build();

        services.AddSmapiDeveloperClient(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>().Value;

        options.ClientId.Should().Be("amzn1.application-oa2-client.example");
        options.ClientSecret.Should().Be("example-secret");
        options.RefreshToken.Should().Be("Atzr|example-refresh-token");
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithPartialConfiguration_LeavesMissingKeysAtTheirDeclaredDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmapiClient:ClientId"] = "amzn1.application-oa2-client.example"
                // ClientSecret and RefreshToken intentionally absent.
            })
            .Build();

        services.AddSmapiDeveloperClient(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>().Value;

        options.ClientId.Should().Be("amzn1.application-oa2-client.example");
        options.ClientSecret.Should().Be(string.Empty);
        options.RefreshToken.Should().Be(string.Empty);
    }

    [Fact]
    public void AddSmapiDeveloperClient_WithCustomSectionName_BindsFromThatSection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AlexaSkillManagement:ClientId"] = "custom-section-client-id",
                ["AlexaSkillManagement:ClientSecret"] = "custom-section-secret",
                ["AlexaSkillManagement:RefreshToken"] = "custom-section-refresh-token"
            })
            .Build();

        services.AddSmapiDeveloperClient(configuration, "AlexaSkillManagement");

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>().Value;

        options.ClientId.Should().Be("custom-section-client-id");
        options.ClientSecret.Should().Be("custom-section-secret");
        options.RefreshToken.Should().Be("custom-section-refresh-token");
    }

    [Fact]
    public void AddSkillInvocationClient_WithConfiguration_BindsEveryPropertyFromConfigurationSection()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["InvocationClient:ClientId"] = "invocation-client-id",
                ["InvocationClient:ClientSecret"] = "invocation-client-secret",
                ["InvocationClient:RefreshToken"] = "invocation-refresh-token"
            })
            .Build();

        services.AddSkillInvocationClient(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>().Value;

        options.ClientId.Should().Be("invocation-client-id");
        options.ClientSecret.Should().Be("invocation-client-secret");
        options.RefreshToken.Should().Be("invocation-refresh-token");
    }

    [Fact]
    public void AddSkillInvocationClient_WithPartialConfiguration_LeavesMissingKeysAtTheirDeclaredDefault()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["InvocationClient:RefreshToken"] = "invocation-refresh-token"
                // ClientId and ClientSecret intentionally absent.
            })
            .Build();

        services.AddSkillInvocationClient(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>().Value;

        options.ClientId.Should().Be(string.Empty);
        options.ClientSecret.Should().Be(string.Empty);
        options.RefreshToken.Should().Be("invocation-refresh-token");
    }
}
