using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class RequestExceptionProcessBehaviorTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithoutException_CallsNextAndReturnsResult(
        RequestExceptionProcessBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(expectedResponse);
        await next.Received(1).Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithExceptionAndMatchingHandler_ReturnsHandledResponse(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IExceptionHandler exceptionHandler,
        SkillResponse handledResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        
        exceptionHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        exceptionHandler.Handle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(handledResponse));
        
        var behavior = new RequestExceptionProcessBehavior(new[] { exceptionHandler });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(handledResponse);
        await exceptionHandler.Received(1).CanHandle(handlerInput, testException, Arg.Any<CancellationToken>());
        await exceptionHandler.Received(1).Handle(handlerInput, testException, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithExceptionAndNonMatchingHandler_RethrowsException(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IExceptionHandler exceptionHandler)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        
        exceptionHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        
        var behavior = new RequestExceptionProcessBehavior(new[] { exceptionHandler });
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
        await exceptionHandler.Received(1).CanHandle(handlerInput, testException, Arg.Any<CancellationToken>());
        await exceptionHandler.DidNotReceive().Handle(Arg.Any<IHandlerInput>(), Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMultipleHandlers_UsesFirstMatchingHandler(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IExceptionHandler firstHandler,
        IExceptionHandler secondHandler,
        SkillResponse firstHandlerResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        
        // First handler can handle the exception
        firstHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        firstHandler.Handle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(firstHandlerResponse));
        
        // Second handler would also be able to handle it but shouldn't be called
        secondHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        
        var behavior = new RequestExceptionProcessBehavior(new[] { firstHandler, secondHandler });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(firstHandlerResponse);
        await firstHandler.Received(1).CanHandle(handlerInput, testException, Arg.Any<CancellationToken>());
        await firstHandler.Received(1).Handle(handlerInput, testException, Arg.Any<CancellationToken>());
        await secondHandler.DidNotReceive().CanHandle(Arg.Any<IHandlerInput>(), Arg.Any<Exception>(), Arg.Any<CancellationToken>());
        await secondHandler.DidNotReceive().Handle(Arg.Any<IHandlerInput>(), Arg.Any<Exception>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMultipleHandlersSecondMatches_UsesSecondHandler(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IExceptionHandler firstHandler,
        IExceptionHandler secondHandler,
        SkillResponse secondHandlerResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        
        // First handler cannot handle the exception
        firstHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        
        // Second handler can handle it
        secondHandler.CanHandle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        secondHandler.Handle(handlerInput, testException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(secondHandlerResponse));
        
        var behavior = new RequestExceptionProcessBehavior(new[] { firstHandler, secondHandler });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(secondHandlerResponse);
        await firstHandler.Received(1).CanHandle(handlerInput, testException, Arg.Any<CancellationToken>());
        await firstHandler.DidNotReceive().Handle(Arg.Any<IHandlerInput>(), Arg.Any<Exception>(), Arg.Any<CancellationToken>());
        await secondHandler.Received(1).CanHandle(handlerInput, testException, Arg.Any<CancellationToken>());
        await secondHandler.Received(1).Handle(handlerInput, testException, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNoExceptionHandlers_RethrowsException(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(testException));
        
        var behavior = new RequestExceptionProcessBehavior(Enumerable.Empty<IExceptionHandler>());
        
        // Act & Assert
        var exception = await Record.ExceptionAsync(() => 
            behavior.Handle(handlerInput, CancellationToken, next));
        
        exception.Should().Be(testException);
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullExceptionHandlers_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => new RequestExceptionProcessBehavior(null!));
        
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithDifferentExceptionTypes_MatchesCorrectHandler(
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        IExceptionHandler argumentHandler,
        IExceptionHandler invalidOpHandler,
        SkillResponse argumentHandlerResponse)
    {
        // Arrange
        var argumentException = new ArgumentException("Argument exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(argumentException));
        
        // Setup handlers for different exception types
        argumentHandler.CanHandle(handlerInput, Arg.Is<ArgumentException>(e => e == argumentException), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        argumentHandler.Handle(handlerInput, argumentException, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(argumentHandlerResponse));
        
        invalidOpHandler.CanHandle(handlerInput, Arg.Is<InvalidOperationException>(e => true), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        
        var behavior = new RequestExceptionProcessBehavior(new[] { argumentHandler, invalidOpHandler });
        
        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        result.Should().Be(argumentHandlerResponse);
        await argumentHandler.Received(1).CanHandle(handlerInput, argumentException, Arg.Any<CancellationToken>());
        await argumentHandler.Received(1).Handle(handlerInput, argumentException, Arg.Any<CancellationToken>());
    }
}