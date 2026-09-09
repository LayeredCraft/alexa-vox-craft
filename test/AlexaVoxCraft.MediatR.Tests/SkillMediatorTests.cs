using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Wrappers;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Tests;

// Send_WithKeyedRegistrationForRequestType_DispatchesViaKeyedServicePath below is the first
// SkillMediatorTests case that dispatches through a real registered handler, which makes
// RequestHandlerWrapperImpl emit real Activity spans on the shared "AlexaVoxCraft" ActivitySource -
// the same source OtelPerformanceLoggingBehaviorTests/OtelRequestHandlerWrapperTests listen to and
// assert against with ContainSingle(). Joining their collection serializes against them so their
// process-wide ActivityListener doesn't capture spans from an unrelated, concurrently-running test.
[Collection("DiagnosticsConfig Tests")]
public class SkillMediatorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException(
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        var exception = Record.Exception(() => new SkillMediator(null!, validConfiguration));

        exception.Should().BeOfType<ArgumentNullException>().Subject.ParamName.Should().Be("serviceProvider");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullConfiguration_ThrowsNullReferenceException(
        IServiceProvider serviceProvider)
    {
        var exception = Record.Exception(() => new SkillMediator(serviceProvider, null!));

        exception.Should().BeOfType<NullReferenceException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullConfigurationValue_ThrowsArgumentNullException(
        IServiceProvider serviceProvider)
    {
        // Arrange - an IOptions<T> whose Value is itself null
        var nullConfigurationValue = TestHelper.SkillOptionsWithNullValue();

        var exception = Record.Exception(() => new SkillMediator(serviceProvider, nullConfigurationValue));

        exception.Should().BeOfType<ArgumentNullException>().Subject.ParamName.Should().Be("serviceConfiguration");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithValidParameters_CreatesInstance(
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        mediator.Should().NotBeNull();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithNullSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var nullConfiguration = TestHelper.SkillOptions(skillId: null);
        var mediator = new SkillMediator(serviceProvider, nullConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithEmptySkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var emptyConfiguration = TestHelper.SkillOptions(skillId: string.Empty);
        var mediator = new SkillMediator(serviceProvider, emptyConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithWhitespaceSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var whitespaceConfiguration = TestHelper.SkillOptions(skillId: "   ");
        var mediator = new SkillMediator(serviceProvider, whitespaceConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithMismatchedSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> invalidConfiguration)
    {
        var mediator = new SkillMediator(serviceProvider, invalidConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithMatchingSkillId_PassesSkillIdValidation(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        skillRequest.Context.System.Application.ApplicationId = validConfiguration.Value.SkillId!;

        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        // Should not be the skill ID verification exception
        exception.Should().NotBeOfType<ArgumentException>();
        exception.Message.Should().NotBe("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_CreatesHandlerWrapper_ThrowsInvalidOperationExceptionForUnsupportedType(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        skillRequest.Context.System.Application.ApplicationId = validConfiguration.Value.SkillId!;

        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        // The exception thrown when trying to create the wrapper for unsupported request types
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Should().BeOfType<InvalidOperationException>().Subject.Message.Should().Contain("Handler was not found for request of type");
    }

    [Fact]
    public async Task Send_WithKeyedRegistrationForRequestType_DispatchesViaKeyedServicePath()
    {
        // Proves the keyed-DI bridge (ADR-0001, plan 0003 Task Group 5): SkillMediator.Send must try
        // GetKeyedService<RequestHandlerWrapper>(requestType) before falling back to the
        // MakeGenericType-based ConcurrentDictionary cache. Registers the keyed factory manually here
        // (production code emits the equivalent call from InterceptorEmitter.EmitHandlerRegistrations)
        // and asserts it - not the fallback path - is what actually produced the wrapper.
        var keyedFactoryInvoked = false;
        var expectedResponse = new SkillResponse { Response = new ResponseBody() };

        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger<SkillMediator>>(NullLogger<SkillMediator>.Instance);
        services.AddTransient<IRequestHandler<LaunchRequest>>(_ => new StubLaunchHandler(expectedResponse));
        services.AddKeyedSingleton<RequestHandlerWrapper>(typeof(LaunchRequest), (_, _) =>
        {
            keyedFactoryInvoked = true;
            return new RequestHandlerWrapperImpl<LaunchRequest>();
        });

        var skillRequest = new SkillRequest
        {
            Request = new LaunchRequest(),
            Context = new Context
            {
                System = new AlexaSystem { Application = new Application { ApplicationId = "amzn1.ask.skill.keyed-test" } }
            }
        };
        services.AddSingleton<IHandlerInput>(new StubHandlerInput(skillRequest));

        var serviceProvider = services.BuildServiceProvider();
        var configuration = Options.Create(new SkillServiceConfiguration { SkillId = "amzn1.ask.skill.keyed-test" });
        var mediator = new SkillMediator(serviceProvider, configuration);

        var response = await mediator.Send(skillRequest, CancellationToken);

        keyedFactoryInvoked.Should().BeTrue();
        response.Should().BeSameAs(expectedResponse);
    }

    private sealed class StubHandlerInput(SkillRequest requestEnvelope) : IHandlerInput
    {
        public SkillRequest RequestEnvelope { get; } = requestEnvelope;
        public IAttributesManager AttributesManager => null!;
        public IResponseBuilder ResponseBuilder => null!;
    }

    private sealed class StubLaunchHandler(SkillResponse response) : IRequestHandler<LaunchRequest>
    {
        public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default) => Task.FromResult(response);
    }
}