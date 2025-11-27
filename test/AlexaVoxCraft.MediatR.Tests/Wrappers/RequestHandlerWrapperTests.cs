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
    [MediatRAutoData]
    public async Task Handle_WithMatchingHandler_CallsHandler(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        handler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);
        
        // Assert
        result.Should().Be(expectedResponse);
        await handler.Received(1).CanHandle(handlerInput, Arg.Any<CancellationToken>());
        await handler.Received(1).Handle(handlerInput, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNonMatchingHandler_CallsDefaultHandler(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        defaultHandler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        defaultHandler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(defaultHandler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);
        
        // Assert
        result.Should().Be(expectedResponse);
        await defaultHandler.Received(1).CanHandle(handlerInput, Arg.Any<CancellationToken>());
        await defaultHandler.Received(1).Handle(handlerInput, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNoMatchingHandlers_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        
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
    [MediatRAutoData]
    public async Task Handle_WithNonMatchingDefaultHandler_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        [Frozen] IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        defaultHandler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        
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
    [MediatRAutoData]
    public async Task Handle_WithPipelineBehaviors_ExecutesBehaviorsInReverseOrder(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        [Frozen] IRequestHandler<LaunchRequest> handler,
        IPipelineBehavior behavior1,
        IPipelineBehavior behavior2)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        handler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        var executionOrder = new List<string>();
        
        behavior1.Handle(handlerInput, Arg.Any<CancellationToken>(), Arg.Any<RequestHandlerDelegate>())
            .Returns(async call =>
            {
                executionOrder.Add("Behavior1-Start");
                var result = await call.Arg<RequestHandlerDelegate>()();
                executionOrder.Add("Behavior1-End");
                return result;
            });
        
        behavior2.Handle(handlerInput, Arg.Any<CancellationToken>(), Arg.Any<RequestHandlerDelegate>())
            .Returns(async call =>
            {
                executionOrder.Add("Behavior2-Start");
                var result = await call.Arg<RequestHandlerDelegate>()();
                executionOrder.Add("Behavior2-End");
                return result;
            });
        
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
    [MediatRAutoData]
    public async Task Handle_MissingHandlerInput_ThrowsInvalidOperationException(
        SkillRequest skillRequest,
        [Frozen] IServiceCollection services)
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