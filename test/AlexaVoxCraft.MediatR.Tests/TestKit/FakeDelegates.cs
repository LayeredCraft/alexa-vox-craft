using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Tests.TestKit;

/// <summary>
/// PLAN-0051 (Compono ecosystem migration, TestKit slice 1, Stage 2): the accepted project-local
/// alternative for <see cref="RequestHandlerDelegate"/> under the current Compono.TestDoubles
/// scope - Compono.TestDoubles deliberately does not generate doubles for delegate types
/// (ADR-0042's Non-Goals: "no support for classes, delegates, indexers, events"), an
/// already-decided, intentional design difference, re-confirmed (not new evidence for a roadmap
/// candidate) during this migration's own ADR-0029 pass. An intentional boundary based on current
/// evidence, not a permanent guarantee - reconsider if real evidence for delegate-double support
/// surfaces later. The implicit conversion to <see cref="RequestHandlerDelegate"/> lets this compose in
/// place of the delegate parameter itself at every call site unchanged; only the
/// configuration/verification lines change from NSubstitute's <c>.Returns()</c>/<c>.Received()</c>
/// to this type's own members.
/// </summary>
public sealed class FakeRequestHandlerDelegate
{
    private Func<Task<SkillResponse>>? _response;

    public int CallCount { get; private set; }

    public static implicit operator RequestHandlerDelegate(FakeRequestHandlerDelegate fake) => fake.Invoke;

    public void Returns(SkillResponse response) => _response = () => Task.FromResult(response);

    public void Returns(Task<SkillResponse> response) => _response = () => response;

    public void Throws(Exception exception) => _response = () => Task.FromException<SkillResponse>(exception);

    private Task<SkillResponse> Invoke()
    {
        CallCount++;
        return _response is not null
            ? _response()
            : throw new InvalidOperationException(
                $"{nameof(FakeRequestHandlerDelegate)} was invoked before {nameof(Returns)}/{nameof(Throws)} configured it.");
    }
}

/// <summary>
/// PLAN-0051 (Compono ecosystem migration, TestKit slice 1, Stage 2): a hand-written fake
/// implementation of <see cref="IPipelineBehavior"/>, unlike <see cref="FakeRequestHandlerDelegate"/>
/// above - <see cref="IPipelineBehavior"/> is an ordinary interface Compono.TestDoubles otherwise
/// supports, not a delegate type (ADR-0042's already-decided delegate non-goal doesn't apply
/// here). The old NSubstitute test
/// (Wrappers/RequestHandlerWrapperTests.cs's Handle_WithPipelineBehaviors_ExecutesBehaviorsInReverseOrder)
/// configured <c>.Returns(async call =&gt; { ...; await call.Arg&lt;RequestHandlerDelegate&gt;()(); ...})</c>
/// - a callback that invokes the captured <see cref="RequestHandlerDelegate"/> argument and
/// records side effects around it. Compono.TestDoubles deliberately has no
/// <c>Returns(Func&lt;CallInfo, T&gt;)</c>-style callback response ("no Returns(Func&lt;...&gt;)
/// callbacks" is an explicit non-goal - docs/packages/compono-testdoubles.md's "What it
/// deliberately doesn't do") - there is no way to attach a side effect to a generated double's
/// member invocation.
///
/// Classified via this migration's own Compono ADR-0029 pass: exactly one real site across this
/// repo's full history ever needed this shape (this test, both behavior1 and behavior2 configured
/// identically). Frequency alone would normally point toward "intentional design difference," but
/// Compono.NSubstitute (a real substitute, callbacks included) CAN satisfy this exact shape where
/// Compono.TestDoubles cannot - per Compono's own ADR-0042 Amendment 2, any real, evidenced
/// Compono.NSubstitute-vs-Compono.TestDoubles capability gap is a roadmap candidate regardless of
/// rarity. Recorded as such in Compono's own ADR-0053 (Proposed, problem-only - no API decided
/// yet) and cross-referenced from RESEARCH-0011. This fake stays the accepted interim workaround
/// while that roadmap item is unresolved, not a permanent verdict - it reproduces exactly the real
/// SUT-relevant behavior under test (recording start/end order around invoking the wrapped
/// delegate) as an ordinary class implementing the interface directly, not a double at all.
/// </summary>
public sealed class FakePipelineBehavior(string name, List<string> executionOrder) : IPipelineBehavior
{
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        executionOrder.Add($"{name}-Start");
        var result = await next();
        executionOrder.Add($"{name}-End");
        return result;
    }
}

/// <summary>
/// Same reasoning as <see cref="FakeRequestHandlerDelegate"/>, for <see cref="SkillRequestFactory"/>.
/// Only ever consumed via composition (<c>TestKit/MediatRTestProfile.cs</c>'s
/// <c>Register&lt;SkillRequestFactory&gt;</c>) in this project - no test body configures or
/// verifies it directly, so it only needs a configurable return, not call tracking.
/// </summary>
public sealed class FakeSkillRequestFactory
{
    private Func<SkillRequest?>? _response;

    public static implicit operator SkillRequestFactory(FakeSkillRequestFactory fake) => fake.Invoke;

    public void Returns(SkillRequest? request) => _response = () => request;

    private SkillRequest? Invoke() =>
        _response is not null
            ? _response()
            : throw new InvalidOperationException(
                $"{nameof(FakeSkillRequestFactory)} was invoked before {nameof(Returns)} configured it.");
}
