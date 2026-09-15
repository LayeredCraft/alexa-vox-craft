using AlexaVoxCraft.Smapi;
using AlexaVoxCraft.Smapi.Auth;
using Compono;
using Compono.XunitV3.Aot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// Profile configuration arguments for both SMAPI config-binding profiles below - a compile-time
/// constant per ADR-0067, so it must be a simple positional record (Compono's Phase 2 direct
/// construction, not runtime reflection).
/// </summary>
public sealed record SmapiCredentials(string ClientId, string ClientSecret, string RefreshToken);

// Scenario 10 (original console app): SMAPI configuration-binding runtime path (Issue #191 coverage
// gap) - both AddSmapiDeveloperClient(IConfiguration, ...) and AddSkillInvocationClient(IConfiguration,
// ...), each with its own real IConfiguration/ServiceCollection (so one call's bound values can't mask
// the other's), bound via AlexaVoxCraft.Smapi.csproj's EnableConfigurationBindingGenerator rather than
// reflection-based ConfigurationBinder.Bind. Phase-2 profiles replace the original's inline setup -
// [Compose<TProfile, TConfig>] passes the credential values in directly as compile-time-constant
// attribute arguments, and each profile does the real IConfiguration/ServiceCollection wiring.

public sealed class SmapiDeveloperClientProfile(SmapiCredentials credentials) : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmapiClient:ClientId"] = credentials.ClientId,
                ["SmapiClient:ClientSecret"] = credentials.ClientSecret,
                ["SmapiClient:RefreshToken"] = credentials.RefreshToken
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSmapiDeveloperClient(configuration);
        var provider = services.BuildServiceProvider();

        builder.Register(() => provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>());
    }
}

public sealed class SkillInvocationClientProfile(SmapiCredentials credentials) : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["InvocationClient:ClientId"] = credentials.ClientId,
                ["InvocationClient:ClientSecret"] = credentials.ClientSecret,
                ["InvocationClient:RefreshToken"] = credentials.RefreshToken
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSkillInvocationClient(configuration);
        var provider = services.BuildServiceProvider();

        builder.Register(() => provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>());
    }
}

public sealed class SmapiConfigBindingTests
{
    [Theory]
    [Compose<SmapiDeveloperClientProfile, SmapiCredentials>(
        "validation-client-id", "validation-client-secret", "validation-refresh-token")]
    public void AddSmapiDeveloperClient_BindsOptions_FromConfiguration(IOptions<SmapiDeveloperAccessTokenOptions> options)
    {
        Assert.Equal("validation-client-id", options.Value.ClientId);
        Assert.Equal("validation-client-secret", options.Value.ClientSecret);
        Assert.Equal("validation-refresh-token", options.Value.RefreshToken);
    }

    [Theory]
    [Compose<SkillInvocationClientProfile, SmapiCredentials>(
        "validation-invocation-client-id", "validation-invocation-client-secret", "validation-invocation-refresh-token")]
    public void AddSkillInvocationClient_BindsOptions_FromConfiguration(IOptions<SmapiDeveloperAccessTokenOptions> options)
    {
        Assert.Equal("validation-invocation-client-id", options.Value.ClientId);
        Assert.Equal("validation-invocation-client-secret", options.Value.ClientSecret);
        Assert.Equal("validation-invocation-refresh-token", options.Value.RefreshToken);
    }
}
