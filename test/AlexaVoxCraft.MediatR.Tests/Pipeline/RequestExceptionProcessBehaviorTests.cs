using Compono;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class RequestExceptionProcessBehaviorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithoutException_CallsNextAndReturnsResult(
        RequestExceptionProcessBehavior behavior,
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        next.CallCount.Should().Be(1);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithExceptionAndMatchingHandler_ReturnsHandledResponse(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IExceptionHandler exceptionHandler,
        SkillResponse handledResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(testException));

        exceptionHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        exceptionHandler.Configure().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(handledResponse));

        var behavior = new RequestExceptionProcessBehavior(new[] { exceptionHandler });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(handledResponse);
        exceptionHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        exceptionHandler.Verify().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithExceptionAndNonMatchingHandler_RethrowsException(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IExceptionHandler exceptionHandler)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(testException));

        exceptionHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        var behavior = new RequestExceptionProcessBehavior(new[] { exceptionHandler });

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(testException);
        exceptionHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        exceptionHandler.Verify().Handle(Match.Any<IHandlerInput>(), Match.Any<Exception>(), Match.Any<CancellationToken>()).Never();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithMultipleHandlers_UsesFirstMatchingHandler(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IExceptionHandler firstHandler,
        IExceptionHandler secondHandler,
        SkillResponse firstHandlerResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(testException));

        // First handler can handle the exception
        firstHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        firstHandler.Configure().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(firstHandlerResponse));

        // Second handler would also be able to handle it but shouldn't be called
        secondHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var behavior = new RequestExceptionProcessBehavior(new[] { firstHandler, secondHandler });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(firstHandlerResponse);
        firstHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        firstHandler.Verify().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        secondHandler.Verify().CanHandle(Match.Any<IHandlerInput>(), Match.Any<Exception>(), Match.Any<CancellationToken>()).Never();
        secondHandler.Verify().Handle(Match.Any<IHandlerInput>(), Match.Any<Exception>(), Match.Any<CancellationToken>()).Never();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithMultipleHandlersSecondMatches_UsesSecondHandler(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IExceptionHandler firstHandler,
        IExceptionHandler secondHandler,
        SkillResponse secondHandlerResponse)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(testException));

        // First handler cannot handle the exception
        firstHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Second handler can handle it
        secondHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        secondHandler.Configure().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(secondHandlerResponse));

        var behavior = new RequestExceptionProcessBehavior(new[] { firstHandler, secondHandler });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(secondHandlerResponse);
        firstHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        firstHandler.Verify().Handle(Match.Any<IHandlerInput>(), Match.Any<Exception>(), Match.Any<CancellationToken>()).Never();
        secondHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
        secondHandler.Verify().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == testException), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNoExceptionHandlers_RethrowsException(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next)
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(testException));

        var behavior = new RequestExceptionProcessBehavior(Enumerable.Empty<IExceptionHandler>());

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(testException);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullExceptionHandlers_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => new RequestExceptionProcessBehavior(null!));

        exception.Should().BeNull();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithDifferentExceptionTypes_MatchesCorrectHandler(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IExceptionHandler argumentHandler,
        IExceptionHandler invalidOpHandler,
        SkillResponse argumentHandlerResponse)
    {
        // Arrange
        var argumentException = new ArgumentException("Argument exception");
        next.Returns(Task.FromException<SkillResponse>(argumentException));

        // Setup handlers for different exception types
        argumentHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == argumentException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        argumentHandler.Configure().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == argumentException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(argumentHandlerResponse));

        invalidOpHandler.Configure().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e is InvalidOperationException), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var behavior = new RequestExceptionProcessBehavior(new[] { argumentHandler, invalidOpHandler });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(argumentHandlerResponse);
        argumentHandler.Verify().CanHandle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == argumentException), Match.Any<CancellationToken>()).Once();
        argumentHandler.Verify().Handle(Match.Is<IHandlerInput>(h => h == handlerInput), Match.Is<Exception>(e => e == argumentException), Match.Any<CancellationToken>()).Once();
    }
}