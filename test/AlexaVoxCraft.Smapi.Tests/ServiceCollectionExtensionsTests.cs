using AlexaVoxCraft.Http;
using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
}
