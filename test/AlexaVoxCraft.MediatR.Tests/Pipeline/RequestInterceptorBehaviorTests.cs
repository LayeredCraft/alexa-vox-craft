using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using AutoFixture.Xunit3;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class RequestInterceptorBehaviorTests : TestBase
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
        var behavior = new RequestInterceptorBehavior(Enumerable.Empty<IRequestInterceptor>());
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSingleInterceptor_ProcessesRequestThenCallsNext(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await interceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await next.Received(1).Invoke();
        
        // Verify interceptor was called before next
        Received.InOrder(async () =>
        {
            await interceptor.Process(handlerInput, Arg.Any<CancellationToken>());
            await next.Invoke();
        });
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMultipleInterceptors_ProcessesAllInOrder(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor firstInterceptor,
        IRequestInterceptor secondInterceptor,
        IRequestInterceptor thirdInterceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { firstInterceptor, secondInterceptor, thirdInterceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await firstInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await secondInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await thirdInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await next.Received(1).Invoke();
        
        // Verify order of execution
        Received.InOrder(async () =>
        {
            await firstInterceptor.Process(handlerInput, Arg.Any<CancellationToken>());
            await secondInterceptor.Process(handlerInput, Arg.Any<CancellationToken>());
            await thirdInterceptor.Process(handlerInput, Arg.Any<CancellationToken>());
            await next.Invoke();
        });
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithInterceptorException_PropagatesException(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor faultyInterceptor,
        IRequestInterceptor normalInterceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Interceptor failed");
        faultyInterceptor.Process(handlerInput, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(testException));
        
        var behavior = new RequestInterceptorBehavior(new[] { faultyInterceptor, normalInterceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await faultyInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await normalInterceptor.DidNotReceive().Process(Arg.Any<IHandlerInput>(), Arg.Any<CancellationToken>());
        await next.DidNotReceive().Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSecondInterceptorException_PropagatesAfterFirst(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor firstInterceptor,
        IRequestInterceptor faultyInterceptor,
        IRequestInterceptor thirdInterceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Second interceptor failed");
        faultyInterceptor.Process(handlerInput, Arg.Any<CancellationToken>())
            .Returns(Task.FromException(testException));
        
        var behavior = new RequestInterceptorBehavior(new[] { firstInterceptor, faultyInterceptor, thirdInterceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await firstInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await faultyInterceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await thirdInterceptor.DidNotReceive().Process(Arg.Any<IHandlerInput>(), Arg.Any<CancellationToken>());
        await next.DidNotReceive().Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullInterceptors_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => new RequestInterceptorBehavior(null!));
        
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithCancellationToken_PassesToInterceptors(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });
        
        // Act
        var result = await behavior.Handle(handlerInput, cancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await interceptor.Received(1).Process(handlerInput, cancellationToken);
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNextException_DoesNotCallInterceptorsAgain(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IRequestInterceptor interceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Next failed");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await interceptor.Received(1).Process(handlerInput, Arg.Any<CancellationToken>());
        await next.Received(1).Invoke();
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
        var emptyInterceptors = new List<IRequestInterceptor>();
        var behavior = new RequestInterceptorBehavior(emptyInterceptors);
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }
}