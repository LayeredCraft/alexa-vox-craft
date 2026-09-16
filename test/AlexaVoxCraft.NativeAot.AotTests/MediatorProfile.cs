using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using Compono;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// The current request a composed <see cref="ISkillMediator"/>'s registered
/// <see cref="SkillRequestFactory"/> reads from - set by a test immediately before calling
/// <c>mediator.Send(...)</c>. <see cref="ThreadStatic"/> because xUnit v3 may run independent
/// [Theory]/[Compose] tests on different threads; each test's own thread only ever sees the value it
/// set itself. Preserved from the original console app's identical <c>file static class
/// AmbientRequest</c> pattern (there it only needed one thread; here it needs to be safe across
/// however many threads xUnit chooses to use).
/// </summary>
internal static class AmbientRequest
{
    [ThreadStatic]
    private static SkillRequest? _current;
    public static SkillRequest? Current { get => _current; set => _current = value; }
}

/// <summary>
/// The skill ID a composed <see cref="ISkillMediator"/> is configured to accept -
/// <see cref="SkillMediator"/> verifies the dispatched <see cref="SkillRequest"/>'s own
/// <c>Context.System.Application.ApplicationId</c> against this exact value and throws if they
/// differ, so this can't be a single hardcoded constant shared by every mediator-dispatch test:
/// Scenario 5 builds its own <see cref="SkillRequest"/> by hand (any ID it likes), while the APL
/// Lambda boundary scenario dispatches a real, trusted fixture carrying its own real application ID -
/// a genuine <c>[Compose&lt;TProfile, TConfig&gt;]</c> use case, not just a contrived one.
/// </summary>
public sealed record MediatorSkillId(string Value);

/// <summary>
/// Composes a real, fully-wired <see cref="ISkillMediator"/> dispatching through the generator-produced
/// keyed dispatch path (<c>AddSkillMediator</c> + the MediatR interceptor generator, exactly as a real
/// skill's <c>Init(IHostBuilder)</c> wires it) - the Phase-2 profile replacing the original console
/// app's inline per-scenario <c>ServiceCollection</c>/<c>ConfigurationBuilder</c> setup (Scenarios 5 and
/// 9). Both <see cref="LaunchHandler"/> and <see cref="UserEventHandler"/> live in this same assembly,
/// so one <c>RegisterServicesFromAssemblyContaining</c> call (anchored on either type) discovers both -
/// this profile is shared by every mediator-dispatch test regardless of which handler a given scenario
/// actually exercises.
/// </summary>
public sealed class MediatorProfile(MediatorSkillId skillId) : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        builder.Register(() => new MediatorTestScope(skillId.Value));
    }
}

/// <summary>
/// Owns the real service provider and scope required for one mediator scenario. Tests dispose this
/// harness after use, ensuring the profile's DI resources cannot outlive the test invocation.
/// </summary>
public sealed class MediatorTestScope : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    public MediatorTestScope(string skillId)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Skill:SkillId"] = skillId })
            .Build();

        var services = new ServiceCollection();
        services.AddSkillMediator(configuration, cfg =>
        {
            cfg.SkillId = skillId;
            cfg.RegisterServicesFromAssemblyContaining<LaunchHandler>();
        });
        services.AddScoped<SkillRequestFactory>(_ => () => AmbientRequest.Current);
        services.AddLogging(b => b.AddConsole());

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
        Mediator = _scope.ServiceProvider.GetRequiredService<ISkillMediator>();
    }

    public ISkillMediator Mediator { get; }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }
}
