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
        builder.Register(() => SmapiOptionsProvider.CreateDeveloperClient(credentials));
    }
}

public sealed class SkillInvocationClientProfile(SmapiCredentials credentials) : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        builder.Register(() => SmapiOptionsProvider.CreateSkillInvocationClient(credentials));
    }
}

/// <summary>
/// Owns the DI container used to validate source-generated SMAPI configuration binding. Profiles
/// register the configured instance; the public constructor exists only for Compono's compile-time
/// plan generation and must never be used at runtime.
/// </summary>
public sealed class SmapiOptionsProvider : IDisposable
{
    private readonly ServiceProvider? _provider;

    public SmapiOptionsProvider() =>
        throw new InvalidOperationException("SmapiOptionsProvider must be supplied by its composition profile.");

    private SmapiOptionsProvider(ServiceProvider provider)
    {
        _provider = provider;
        Options = provider.GetRequiredService<IOptions<SmapiDeveloperAccessTokenOptions>>();
    }

    public IOptions<SmapiDeveloperAccessTokenOptions> Options { get; } = null!;

    public static SmapiOptionsProvider CreateDeveloperClient(SmapiCredentials credentials) =>
        Create(CreateConfiguration("SmapiClient", credentials), static (services, configuration) =>
            services.AddSmapiDeveloperClient(configuration));

    public static SmapiOptionsProvider CreateSkillInvocationClient(SmapiCredentials credentials) =>
        Create(CreateConfiguration("InvocationClient", credentials), static (services, configuration) =>
            services.AddSkillInvocationClient(configuration));

    public void Dispose() => _provider?.Dispose();

    private static SmapiOptionsProvider Create(
        IConfiguration configuration,
        Action<IServiceCollection, IConfiguration> registerClient)
    {
        var services = new ServiceCollection();
        registerClient(services, configuration);
        return new SmapiOptionsProvider(services.BuildServiceProvider());
    }

    private static IConfigurationRoot CreateConfiguration(string section, SmapiCredentials credentials) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{section}:ClientId"] = credentials.ClientId,
                [$"{section}:ClientSecret"] = credentials.ClientSecret,
                [$"{section}:RefreshToken"] = credentials.RefreshToken
            })
            .Build();
}

public sealed class SmapiConfigBindingTests
{
    [Theory]
    [Compose<SmapiDeveloperClientProfile, SmapiCredentials>(
        "validation-client-id", "validation-client-secret", "validation-refresh-token")]
    public void AddSmapiDeveloperClient_BindsOptions_FromConfiguration(SmapiOptionsProvider optionsProvider)
    {
        using var _ = optionsProvider;
        Assert.Equal("validation-client-id", optionsProvider.Options.Value.ClientId);
        Assert.Equal("validation-client-secret", optionsProvider.Options.Value.ClientSecret);
        Assert.Equal("validation-refresh-token", optionsProvider.Options.Value.RefreshToken);
    }

    [Theory]
    [Compose<SkillInvocationClientProfile, SmapiCredentials>(
        "validation-invocation-client-id", "validation-invocation-client-secret", "validation-invocation-refresh-token")]
    public void AddSkillInvocationClient_BindsOptions_FromConfiguration(SmapiOptionsProvider optionsProvider)
    {
        using var _ = optionsProvider;
        Assert.Equal("validation-invocation-client-id", optionsProvider.Options.Value.ClientId);
        Assert.Equal("validation-invocation-client-secret", optionsProvider.Options.Value.ClientSecret);
        Assert.Equal("validation-invocation-refresh-token", optionsProvider.Options.Value.RefreshToken);
    }
}
