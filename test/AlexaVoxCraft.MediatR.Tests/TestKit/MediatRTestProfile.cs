using System.Text.Json;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Request.Type;
using Compono;
using Compono.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Tests.TestKit;

/// <summary>
/// Shared composition profile for AlexaVoxCraft.MediatR.Tests.
/// Applied via <c>[Compose&lt;MediatRTestProfile&gt;]</c>.
///
/// Uses Compono.TestDoubles' <c>UseGeneratedTestDoubles()</c> for interface dependencies.
/// The two delegate types this project needs (<see cref="RequestHandlerDelegate"/>,
/// <see cref="SkillRequestFactory"/>) use <c>FakeDelegates.cs</c> instead because
/// Compono.TestDoubles deliberately does not generate doubles for delegate types.
/// </summary>
public sealed class MediatRTestProfile : ICompositionProfile
{
    public void Configure(CompositionBuilder builder) =>
        builder
            // UseLogging() registered before UseGeneratedTestDoubles() (stage-6 first-registered-
            // wins, ADR-0055) - PerformanceLoggingBehavior's ILogger<T> asserts observable log
            // output, so it needs Compono.Logging's CapturingLogger<T>, not a generated double of
            // the ILogger<T> interface shape. Every other ILogger<T> constructor dependency in
            // this project is never itself asserted against, so this ordering doesn't change their
            // behavior (they'd resolve to a CapturingLogger<T> too now, which is fine - unused).
            .UseLogging(options => options.MinimumLevel = LogLevel.Debug)
            // docs/adr/0056-composition-builder-share-graph-wide-sharing.md: every request for
            // ILogger<PerformanceLoggingBehavior> anywhere in the graph now participates
            // automatically - PerformanceLoggingBehaviorTests.cs's own `logger` theory parameters
            // no longer need [Shared] to observe the exact instance PerformanceLoggingBehavior
            // itself logs through.
            .Share<ILogger<PerformanceLoggingBehavior>>()
            .UseGeneratedTestDoubles()
            // Every real theory in this project composes SkillResponse directly somewhere in the
            // project (a compile-time discovery root), so this nested context.Resolve<SkillResponse>()
            // is not RESEARCH-0010 Finding B's shape - included here only because an unconfigured
            // FakeRequestHandlerDelegate has no return value by default.
            .Register<RequestHandlerDelegate>(context =>
            {
                var next = new FakeRequestHandlerDelegate();
                next.Returns(context.Resolve<SkillResponse>());
                return next;
            })
            // Same reasoning as above - SkillRequest is composed directly elsewhere in this
            // project (e.g. DefaultHandlerInputTests' own SkillRequest parameters), so this is
            // not Finding B's shape either.
            .Register<SkillRequestFactory>(context =>
            {
                var factory = new FakeSkillRequestFactory();
                factory.Returns(context.Resolve<SkillRequest>());
                return factory;
            })
            // ServiceCollection/ServiceProvider need to be *real*, stateful DI container objects
            // (AddSingleton/BuildServiceProvider must actually work), not a test-double provider's
            // generated fake of the IServiceCollection/IServiceProvider interface shape; tests
            // exercise AddSingleton/BuildServiceProvider behavior directly for these two types.
            .Register<IServiceCollection>(() => new ServiceCollection())
            // Correction to an earlier (incorrect) comment here claiming ILogger<SkillMediator>
            // "resolves cleanly" as a nested context.Resolve<T>() with no registration - a real
            // isolated run of SkillMediatorTests proved otherwise: no test anywhere in this project
            // requests ILogger<SkillMediator> as its own theory parameter (a discovery root), so no
            // generated test-double closure ever reaches it, and the nested
            // context.Resolve<ILogger<SkillMediator>>() below genuinely threw CompositionException
            // ("no ... test-double provider ... could satisfy"). SkillMediator's own logger is never
            // asserted against by any test here (unlike PerformanceLoggingBehavior's CapturingLogger<T>
            // above), so a plain NullLogger<T> is the right fallback, not a generated double.
            .Register<ILogger<SkillMediator>>(() => Microsoft.Extensions.Logging.Abstractions.NullLogger<SkillMediator>.Instance)
            .Register<IServiceProvider>(context =>
            {
                var services = new ServiceCollection();
                services.AddSingleton(context.Resolve<ILogger<SkillMediator>>());
                services.AddSingleton(context.Resolve<IHandlerInput>());
                return services.BuildServiceProvider();
            })
            // IAttributesManager.Session (JsonAttributeBag, no deterministic default - ADR-0045
            // configuration-required) is dereferenced by DefaultResponseBuilder.GetResponse() on
            // every real call (it persists response-scoped state there), even for tests that never
            // touch attributes at all (the large majority of DefaultResponseBuilderTests' own
            // Speak/Reprompt/*Card/AudioPlayer cases). Constructing the double directly via
            // GeneratedTestDoubleRegistry.TryCreate and pre-configuring Session here (not a
            // recursive context.Resolve<IAttributesManager>() - would be circular) gives every
            // composed IAttributesManager a real, empty, non-throwing bag by default; a test that
            // cares about attribute contents still calls its own
            // attributesManager.Configure().Session().Returns(...) afterward, which wins over this
            // default per ADR-0050's last-registration-wins multi-entry semantics.
            .Register<IAttributesManager>(() =>
            {
                GeneratedTestDoubleRegistry.TryCreate(typeof(IAttributesManager), out var value);
                var attributesManager = (IAttributesManager)value!;
                attributesManager.Configure().Session().Returns(new JsonAttributeBag(new Dictionary<string, JsonElement>()));
                return attributesManager;
            })
            // No Register<IHandlerInput> here. Compono.TestDoubles can't be preconfigured from
            // inside a Register<T> factory - context.Resolve<IHandlerInput>() for the very
            // type being registered would be circular - and doesn't need to be: RequestEnvelope
            // (a non-nullable SkillRequest return with no deterministic default) generates as
            // *configuration-required* (ADR-0045) instead of silently null. Every test that
            // dereferences handlerInput.RequestEnvelope now calls
            // handlerInput.Configure().RequestEnvelope().Returns(...) explicitly - more honest
            // than implicit auto-population or silent-null defaults, at the cost of one explicit
            // line per test that needs it.
            //
            // IOptions<SkillServiceConfiguration>.Value: unlike IHandlerInput above, the old
            // OptionsSpecimenBuilder was NOT a blanket ConfigureMembers default - it inspected the
            // requesting parameter's *name* (e.g. "emptyConfiguration" -> SkillId="",
            // "whitespaceConfiguration" -> SkillId="   ", "nullConfigurationValue" -> null Value)
            // to hand back scenario-specific data. That's exactly the kind of magic-string-keyed
            // implicit behavior Compono deliberately has no equivalent for (no member/type rule is
            // keyed by the requesting parameter's name) - correctly so, per explicit-over-implicit.
            // This registration supplies only the generic case (the majority of real call sites,
            // which just need *a* non-null, auto-composed SkillServiceConfiguration and don't care
            // about its exact field values); the handful of SkillMediatorTests/DefaultResponseBuilderTests
            // cases that genuinely need a specific SkillId/DefaultVoiceName now build that value
            // explicitly in their own Arrange section instead of relying on a parameter-name match
            // - see those files' own comments for the specific tests.
            // SkillServiceConfiguration's own properties (SkillId/CustomUserAgent/DefaultVoiceName)
            // are plain settable strings, not constructor parameters or `required` members - same
            // reason as SkillResponse.Response below, this stayed null under plain generated
            // default construction until given its own explicit default here. Matches the old
            // SkillServiceConfigurationSpecimenBuilder's own generic fallback
            // (CreateDefaultConfiguration) - a non-null, matchable SkillId, not a magic/random one.
            .Register<SkillServiceConfiguration>(_ => new SkillServiceConfiguration
            {
                SkillId = "amzn1.ask.skill.default-test-id",
                CustomUserAgent = "TestAgent/1.0",
            })
            .Register<IOptions<SkillServiceConfiguration>>(context =>
                Options.Create(context.Resolve<SkillServiceConfiguration>()))
            // SkillRequest.Request is `abstract class Request` - a provider-resolved leaf with no
            // registration, so plain auto-composition always leaves it null. The old
            // SkillRequestSpecimenBuilder was itself parameter-name-keyed (launch/intent/help/
            // session-end/audio/display/playback/system), but its own genuine fallback - and the
            // shape every "don't care which subtype" test actually needs - was always a valid
            // LaunchRequest (see its own `_ => CreateLaunchRequest()` default). Classified ADR-0029
            // "Acceptable Compono-native alternative" alongside the two Register<T> rules above -
            // this default covers every generic call site; the handful of tests that need a
            // *specific* Request subtype build it explicitly via TestHelper instead of relying on
            // a parameter name (see TestHelper.cs and the affected test files' own comments).
            .Register<SkillRequest>(_ => TestHelper.ForRequest(new LaunchRequest
            {
                Type = "LaunchRequest",
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Locale = "en-US",
            }))
            // SkillResponse.Response is a plain settable ResponseBody property, not a constructor
            // parameter or a `required` member - Compono's generated default construction (per
            // docs/concepts/composition-model.md: "the type's own shape (constructor, required
            // members)") only sets constructor parameters/required members, not ordinary settable
            // properties, so it stays null. Compono deliberately does not populate every public
            // settable property. Project-local default, same shape as
            // SkillRequest above - ResponseBody's own nullable members (OutputSpeech/Card/Reprompt)
            // and field-initialized Directives/ShouldEndSession are already safe at their own
            // defaults, so only Response itself needs to be non-null.
            .Register<SkillResponse>(_ => new SkillResponse
            {
                Version = "1.0",
                Response = new ResponseBody(),
            });
}
