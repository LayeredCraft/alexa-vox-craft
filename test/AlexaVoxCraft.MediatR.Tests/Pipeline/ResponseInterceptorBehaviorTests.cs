using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using AutoFixture.Xunit3;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class ResponseInterceptorBehaviorTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNoInterceptors_CallsNextAndReturnsResult(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new ResponseInterceptorBehavior(Enumerable.Empty<IResponseInterceptor>());
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSingleInterceptor_CallsNextThenProcessesResponse(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new ResponseInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
        await interceptor.Received(1).Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        
        // Verify next was called before interceptor
        Received.InOrder(async () =>
        {
            await next.Invoke();
            await interceptor.Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        });
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMultipleInterceptors_ProcessesAllInOrder(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor firstInterceptor,
        IResponseInterceptor secondInterceptor,
        IResponseInterceptor thirdInterceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new ResponseInterceptorBehavior(new[] { firstInterceptor, secondInterceptor, thirdInterceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
        await firstInterceptor.Received(1).Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        await secondInterceptor.Received(1).Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        await thirdInterceptor.Received(1).Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        
        // Verify order of execution
        Received.InOrder(async () =>
        {
            await next.Invoke();
            await firstInterceptor.Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
            await secondInterceptor.Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
            await thirdInterceptor.Process(handlerInput, expectedResponse, Arg.Any<CancellationToken>());
        });
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNextException_DoesNotCallInterceptors(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor interceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Next failed");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        var behavior = new ResponseInterceptorBehavior(new[] { interceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await next.Received(1).Invoke();
        await interceptor.DidNotReceive().Process(Arg.Any<IHandlerInput>(), Arg.Any<SkillResponse>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithInterceptorException_PropagatesException(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor faultyInterceptor,
        IResponseInterceptor normalInterceptor,
        SkillResponse response)
    {
        // Arrange
        var testException = new InvalidOperationException("Interceptor failed");
        next.Invoke().Returns(Task.FromResult(response));
        faultyInterceptor.Process(handlerInput, response, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(testException));
        
        var behavior = new ResponseInterceptorBehavior(new[] { faultyInterceptor, normalInterceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await next.Received(1).Invoke();
        await faultyInterceptor.Received(1).Process(handlerInput, response, Arg.Any<CancellationToken>());
        await normalInterceptor.DidNotReceive().Process(Arg.Any<IHandlerInput>(), Arg.Any<SkillResponse>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSecondInterceptorException_PropagatesAfterFirst(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor firstInterceptor,
        IResponseInterceptor faultyInterceptor,
        IResponseInterceptor thirdInterceptor,
        SkillResponse response)
    {
        // Arrange
        var testException = new InvalidOperationException("Second interceptor failed");
        next.Invoke().Returns(Task.FromResult(response));
        faultyInterceptor.Process(handlerInput, response, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(testException));
        
        var behavior = new ResponseInterceptorBehavior(new[] { firstInterceptor, faultyInterceptor, thirdInterceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await next.Received(1).Invoke();
        await firstInterceptor.Received(1).Process(handlerInput, response, Arg.Any<CancellationToken>());
        await faultyInterceptor.Received(1).Process(handlerInput, response, Arg.Any<CancellationToken>());
        await thirdInterceptor.DidNotReceive().Process(Arg.Any<IHandlerInput>(), Arg.Any<SkillResponse>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullInterceptors_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => new ResponseInterceptorBehavior(null!));
        
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithCancellationToken_PassesToInterceptors(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new ResponseInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, cancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await interceptor.Received(1).Process(handlerInput, expectedResponse, cancellationToken);
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_ReturnsOriginalResponse_EvenAfterInterceptorProcessing(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor interceptor,
        SkillResponse originalResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(originalResponse));
        var behavior = new ResponseInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(originalResponse);
        await interceptor.Received(1).Process(handlerInput, originalResponse, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithEmptyEnumerable_WorksCorrectly(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var emptyInterceptors = new List<IResponseInterceptor>();
        var behavior = new ResponseInterceptorBehavior(emptyInterceptors);
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithResponseModification_StillReturnsOriginalResponse(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IResponseInterceptor interceptor,
        SkillResponse originalResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(originalResponse));
        
        // Setup interceptor to potentially modify response (but behavior should still return original)
        interceptor.Process(handlerInput, originalResponse, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ => originalResponse.Version = "modified"); // Simulate modification
        
        var behavior = new ResponseInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(originalResponse);
        // Response interceptors can modify the response object, but the behavior returns the same reference
        result.Version.Should().Be("modified");
    }
}