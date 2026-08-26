using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.MediatR.Lambda.Tests.TestKit;
using Compono.XunitV3;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.Context;

public class DefaultSkillContextFactoryTests : TestBase
{
    [Theory, Compose<LambdaTestProfile>]
    public void Constructor_WithValidAccessor_InitializesCorrectly(FakeSkillContextAccessor accessor)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        factory.Should().NotBeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_WithValidRequest_ReturnsDefaultSkillContext(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        var context = factory.Create(skillRequest);

        context.Should().NotBeNull();
        context.Should().BeOfType<DefaultSkillContext>();
        context.Request.Should().Be(skillRequest);
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_SetsContextInAccessor(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        var context = factory.Create(skillRequest);

        accessor.Assignments.Should().ContainSingle().Which.Should().BeSameAs(context);
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_WithNullAccessor_DoesNotThrow(SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(null!);

        var exception = Record.Exception(() => factory.Create(skillRequest));

        exception.Should().BeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_WithNullAccessor_ReturnsValidContext(SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(null!);

        var context = factory.Create(skillRequest);

        context.Should().NotBeNull();
        context.Request.Should().Be(skillRequest);
    }

    [Fact]
    public void Create_WithNullRequest_DoesNotThrow()
    {
        var accessor = new FakeSkillContextAccessor();
        var factory = new DefaultSkillContextFactory(accessor);

        var exception = Record.Exception(() => factory.Create(null!));

        exception.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullRequest_ReturnsContextWithNullRequest()
    {
        var accessor = new FakeSkillContextAccessor();
        var factory = new DefaultSkillContextFactory(accessor);

        var context = factory.Create(null!);

        context.Should().NotBeNull();
        context.Request.Should().BeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Dispose_WithValidAccessor_ClearsContextInAccessor(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        var context = factory.Create(skillRequest);

        factory.Dispose(context);

        accessor.Assignments.Where(x => x is null).Should().ContainSingle();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Dispose_WithNullAccessor_DoesNotThrow(SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(null!);
        var context = factory.Create(skillRequest);

        var exception = Record.Exception(() => factory.Dispose(context));

        exception.Should().BeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Dispose_WithNullContext_DoesNotThrow(FakeSkillContextAccessor accessor)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        var exception = Record.Exception(() => factory.Dispose(null!));

        exception.Should().BeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void CreateAndDispose_Lifecycle_WorksCorrectly(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        // Create context
        var context = factory.Create(skillRequest);

        // Verify creation
        context.Should().NotBeNull();
        accessor.Assignments.Should().ContainSingle().Which.Should().BeSameAs(context);

        // Dispose context
        factory.Dispose(context);

        // Verify disposal
        accessor.Assignments.Where(x => x is null).Should().NotBeEmpty();
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_MultipleCalls_EachSetsContextInAccessor(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest1,
        SkillRequest skillRequest2)
    {
        var factory = new DefaultSkillContextFactory(accessor);

        var context1 = factory.Create(skillRequest1);
        var context2 = factory.Create(skillRequest2);

        context1.Should().NotBe(context2);
        accessor.Assignments.Should().Contain(context1);
        accessor.Assignments.Should().Contain(context2);
        accessor.Assignments.Should().HaveCount(2);
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_PreservesRequestData(SkillRequest skillRequest)
    {
        var accessor = new FakeSkillContextAccessor();
        var factory = new DefaultSkillContextFactory(accessor);

        var context = factory.Create(skillRequest);

        context.Request.Should().BeSameAs(skillRequest);
        context.Request.Request.Type.Should().Be(skillRequest.Request.Type);
        context.Request.Context.Should().Be(skillRequest.Context);
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Dispose_MultipleCallsWithSameContext_OnlySetNullOnce(
        FakeSkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        var context = factory.Create(skillRequest);

        factory.Dispose(context);
        factory.Dispose(context);

        // Should set to null once per dispose call
        accessor.Assignments.Where(x => x is null).Should().HaveCount(2);
    }

    [Theory, Compose<LambdaTestProfile>]
    public void Create_WithDifferentRequestTypes_HandlesAllCorrectly(
        FakeSkillContextAccessor accessor,
        SkillRequest launchRequest,
        SkillRequest intentRequest,
        SkillRequest sessionEndRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        launchRequest = LambdaTestProfile.CreateSkillRequest(new AlexaVoxCraft.Model.Request.Type.LaunchRequest { Type = "LaunchRequest", Locale = "en-US" });
        intentRequest = LambdaTestProfile.CreateSkillRequest(new AlexaVoxCraft.Model.Request.Type.IntentRequest { Type = "IntentRequest", Locale = "en-US" });
        sessionEndRequest = LambdaTestProfile.CreateSkillRequest(new AlexaVoxCraft.Model.Request.Type.SessionEndedRequest { Type = "SessionEndedRequest", Locale = "en-US" });

        var launchContext = factory.Create(launchRequest);
        var intentContext = factory.Create(intentRequest);
        var sessionEndContext = factory.Create(sessionEndRequest);

        launchContext.Request.Request.Type.Should().Be("LaunchRequest");
        intentContext.Request.Request.Type.Should().Be("IntentRequest");
        sessionEndContext.Request.Request.Type.Should().Be("SessionEndedRequest");
    }
}
