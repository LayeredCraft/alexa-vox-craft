using Compono;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class RequestInterceptorBehaviorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNoInterceptors_CallsNextAndReturnsResult(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(Enumerable.Empty<IRequestInterceptor>());

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        next.CallCount.Should().Be(1);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithSingleInterceptor_ProcessesRequestThenCallsNext(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        interceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        next.CallCount.Should().Be(1);

        // GAP (ADR-0029, recorded in RESEARCH-0011 Stage 2): no Compono.TestDoubles call-order
        // verification equivalent exists (explicit non-goal) - see
        // ResponseInterceptorBehaviorTests.cs's matching comment. Each call is still verified
        // exactly once above; their relative order is not currently verifiable.
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithMultipleInterceptors_ProcessesAllInOrder(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor firstInterceptor,
        IRequestInterceptor secondInterceptor,
        IRequestInterceptor thirdInterceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { firstInterceptor, secondInterceptor, thirdInterceptor });

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        firstInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        secondInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        thirdInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        next.CallCount.Should().Be(1);

        // GAP (ADR-0029, recorded in RESEARCH-0011 Stage 2) - same as
        // Handle_WithSingleInterceptor_ProcessesRequestThenCallsNext above.
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithInterceptorException_PropagatesException(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor faultyInterceptor,
        IRequestInterceptor normalInterceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Interceptor failed");
        faultyInterceptor.Configure().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>())
            .Returns(Task.FromException(testException));

        var behavior = new RequestInterceptorBehavior(new[] { faultyInterceptor, normalInterceptor });

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(testException);
        faultyInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        normalInterceptor.Verify().Process(Match.Any<IHandlerInput>(), Match.Any<CancellationToken>()).Never();
        next.CallCount.Should().Be(0);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithSecondInterceptorException_PropagatesAfterFirst(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor firstInterceptor,
        IRequestInterceptor faultyInterceptor,
        IRequestInterceptor thirdInterceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Second interceptor failed");
        faultyInterceptor.Configure().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>())
            .Returns(Task.FromException(testException));

        var behavior = new RequestInterceptorBehavior(new[] { firstInterceptor, faultyInterceptor, thirdInterceptor });

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(testException);
        firstInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        faultyInterceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        thirdInterceptor.Verify().Process(Match.Any<IHandlerInput>(), Match.Any<CancellationToken>()).Never();
        next.CallCount.Should().Be(0);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullInterceptors_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => new RequestInterceptorBehavior(null!));

        exception.Should().BeNull();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithCancellationToken_PassesToInterceptors(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor interceptor,
        SkillResponse expectedResponse)
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        next.Returns(Task.FromResult(expectedResponse));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });

        // Act
        var result = await behavior.Handle(handlerInput, cancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        interceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Is<CancellationToken>(t => t == cancellationToken)).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNextException_DoesNotCallInterceptorsAgain(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        IRequestInterceptor interceptor)
    {
        // Arrange
        var testException = new InvalidOperationException("Next failed");
        next.Returns(Task.FromException<SkillResponse>(testException));
        var behavior = new RequestInterceptorBehavior(new[] { interceptor });

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(testException);
        interceptor.Verify().Process(Match.Is<IHandlerInput>(x => x == handlerInput), Match.Any<CancellationToken>()).Once();
        next.CallCount.Should().Be(1);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithEmptyEnumerable_WorksCorrectly(
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Returns(Task.FromResult(expectedResponse));
        var emptyInterceptors = new List<IRequestInterceptor>();
        var behavior = new RequestInterceptorBehavior(emptyInterceptors);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);
        next.CallCount.Should().Be(1);
    }
}