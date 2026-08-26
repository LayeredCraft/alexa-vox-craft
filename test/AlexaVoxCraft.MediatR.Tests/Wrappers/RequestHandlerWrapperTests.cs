using Compono;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.MediatR.Wrappers;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Tests.Wrappers;

[Collection("DiagnosticsConfig Tests")]
public class RequestHandlerWrapperTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithMatchingHandler_CallsHandler(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Shared] IHandlerInput handlerInput,
        [Shared] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(true));
        handler.Configure().Handle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(expectedResponse));

        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);

        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        handler.Verify().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        handler.Verify().Handle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNonMatchingHandler_CallsDefaultHandler(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Shared] IHandlerInput handlerInput,
        [Shared] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        handler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(false));
        defaultHandler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(true));
        defaultHandler.Configure().Handle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(expectedResponse));

        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(defaultHandler);

        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        defaultHandler.Verify().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        defaultHandler.Verify().Handle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNoMatchingHandlers_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Shared] IHandlerInput handlerInput,
        [Shared] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(false));

        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        // No default handler registered

        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));

        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("Handler was not found for request of type");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNonMatchingDefaultHandler_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Shared] IHandlerInput handlerInput,
        [Shared] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        [Shared] IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        handler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(false));
        defaultHandler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(false));

        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(defaultHandler);

        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));

        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("Handler was not found for request of type");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithPipelineBehaviors_ExecutesBehaviorsInReverseOrder(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Shared] IHandlerInput handlerInput,
        [Shared] IServiceCollection services,
        [Shared] IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.Configure().CanHandle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(true));
        handler.Configure().Handle(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Returns(Task.FromResult(expectedResponse));

        // GAP (ADR-0029, recorded in RESEARCH-0011 Stage 2): Compono.TestDoubles has no callback
        // response equivalent (no Returns(Func<CallInfo, T>), an explicit non-goal), so a
        // TestDoubles-generated IPipelineBehavior double can't invoke the RequestHandlerDelegate
        // argument passed to it. FakePipelineBehavior (TestKit/FakeDelegates.cs) is a small
        // hand-written IPipelineBehavior implementation reproducing exactly the behavior this test
        // needs - not a double at all, an ordinary class.
        var executionOrder = new List<string>();
        IPipelineBehavior behavior1 = new FakePipelineBehavior("Behavior1", executionOrder);
        IPipelineBehavior behavior2 = new FakePipelineBehavior("Behavior2", executionOrder);

        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(behavior2);
        services.AddSingleton(behavior1);

        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        // Behaviors should execute in reverse order (behavior2 wraps behavior1)
        executionOrder.Should().ContainInOrder("Behavior2-Start", "Behavior1-Start", "Behavior1-End", "Behavior2-End");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_MissingHandlerInput_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Shared] IServiceCollection services)
    {
        // Arrange - Intentionally not registering IHandlerInput
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));

        exception.Should().BeOfType<InvalidOperationException>();
    }
}