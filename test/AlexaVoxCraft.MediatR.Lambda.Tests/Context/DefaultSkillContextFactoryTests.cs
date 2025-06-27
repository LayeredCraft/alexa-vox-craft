using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.Model.Request;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.Context;

public class DefaultSkillContextFactoryTests : TestBase
{
    [Theory]
    [AutoData]
    public void Constructor_WithValidAccessor_InitializesCorrectly(ISkillContextAccessor accessor)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        factory.Should().NotBeNull();
    }

    [Theory]
    [AutoData]
    public void Create_WithValidRequest_ReturnsDefaultSkillContext(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        var context = factory.Create(skillRequest);
        
        context.Should().NotBeNull();
        context.Should().BeOfType<DefaultSkillContext>();
        context.Request.Should().Be(skillRequest);
    }

    [Theory]
    [AutoData]
    public void Create_SetsContextInAccessor(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        var context = factory.Create(skillRequest);
        
        accessor.Received(1).SkillContext = context;
    }

    [Theory]
    [AutoData]
    public void Create_WithNullAccessor_DoesNotThrow(SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(null!);
        
        var exception = Record.Exception(() => factory.Create(skillRequest));
        
        exception.Should().BeNull();
    }

    [Theory]
    [AutoData]
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
        var accessor = CreateSubstitute<ISkillContextAccessor>();
        var factory = new DefaultSkillContextFactory(accessor);
        
        var exception = Record.Exception(() => factory.Create(null!));
        
        exception.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullRequest_ReturnsContextWithNullRequest()
    {
        var accessor = CreateSubstitute<ISkillContextAccessor>();
        var factory = new DefaultSkillContextFactory(accessor);
        
        var context = factory.Create(null!);
        
        context.Should().NotBeNull();
        context.Request.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public void Dispose_WithValidAccessor_ClearsContextInAccessor(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        var context = factory.Create(skillRequest);
        
        factory.Dispose(context);
        
        accessor.Received(1).SkillContext = null;
    }

    [Theory]
    [AutoData]
    public void Dispose_WithNullAccessor_DoesNotThrow(SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(null!);
        var context = factory.Create(skillRequest);
        
        var exception = Record.Exception(() => factory.Dispose(context));
        
        exception.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public void Dispose_WithNullContext_DoesNotThrow(ISkillContextAccessor accessor)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        var exception = Record.Exception(() => factory.Dispose(null!));
        
        exception.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public void CreateAndDispose_Lifecycle_WorksCorrectly(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        // Create context
        var context = factory.Create(skillRequest);
        
        // Verify creation
        context.Should().NotBeNull();
        accessor.Received(1).SkillContext = context;
        
        // Dispose context
        factory.Dispose(context);
        
        // Verify disposal
        accessor.Received(1).SkillContext = null;
    }

    [Theory]
    [AutoData]
    public void Create_MultipleCalls_EachSetsContextInAccessor(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest1,
        SkillRequest skillRequest2)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        var context1 = factory.Create(skillRequest1);
        var context2 = factory.Create(skillRequest2);
        
        context1.Should().NotBe(context2);
        accessor.Received(1).SkillContext = context1;
        accessor.Received(1).SkillContext = context2;
        accessor.Received(2).SkillContext = Arg.Any<SkillContext>();
    }

    [Theory]
    [AutoData]
    public void Create_PreservesRequestData(SkillRequest skillRequest)
    {
        var accessor = CreateSubstitute<ISkillContextAccessor>();
        var factory = new DefaultSkillContextFactory(accessor);
        
        var context = factory.Create(skillRequest);
        
        context.Request.Should().BeSameAs(skillRequest);
        context.Request.Request.Type.Should().Be(skillRequest.Request.Type);
        context.Request.Context.Should().Be(skillRequest.Context);
    }

    [Theory]
    [AutoData]
    public void Dispose_MultipleCallsWithSameContext_OnlySetNullOnce(
        ISkillContextAccessor accessor,
        SkillRequest skillRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        var context = factory.Create(skillRequest);
        
        factory.Dispose(context);
        factory.Dispose(context);
        
        // Should only set to null once per dispose call
        accessor.Received(2).SkillContext = null;
    }

    [Theory]
    [AutoData]
    public void Create_WithDifferentRequestTypes_HandlesAllCorrectly(
        ISkillContextAccessor accessor,
        SkillRequest launchRequest,
        SkillRequest intentRequest,
        SkillRequest sessionEndRequest)
    {
        var factory = new DefaultSkillContextFactory(accessor);
        
        var launchContext = factory.Create(launchRequest);
        var intentContext = factory.Create(intentRequest);
        var sessionEndContext = factory.Create(sessionEndRequest);
        
        launchContext.Request.Request.Type.Should().Be("LaunchRequest");
        intentContext.Request.Request.Type.Should().Be("IntentRequest");
        sessionEndContext.Request.Request.Type.Should().Be("SessionEndedRequest");
    }
}