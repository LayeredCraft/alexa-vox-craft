using Compono;
using Compono.Logging;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

[Collection("DiagnosticsConfig Tests")]
public class PerformanceLoggingBehaviorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithSuccessfulRequest_LogsDebugMessages(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest skillRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(skillRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        result.Should().Be(expectedResponse);

        // Verify logging using Compono.Logging
        logger.GetCapturedEntries().Count(e => e.LogLevel == LogLevel.Debug).Should().Be(2);
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Processing Alexa skill request").Once();
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Successfully processed Alexa skill request").Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithException_LogsErrorAndRethrows(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillRequest skillRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(skillRequest);
        var expectedException = new InvalidOperationException("Test exception");
        next.Returns(Task.FromException<SkillResponse>(expectedException));

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            behavior.Handle(handlerInput, CancellationToken, next));

        exception.Should().Be(expectedException);

        // Verify error logging using Compono.Logging
        logger.GetCapturedEntries().Count(e => e.LogLevel == LogLevel.Error).Should().Be(1);
        logger.Verify()
            .AtLevel(LogLevel.Error)
            .WithMessageContaining("Failed to process Alexa skill request")
            .WithException<InvalidOperationException>()
            .Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithIntentRequest_LogsIntentName(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Shared] IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        var intentRequest = TestHelper.ForRequest(TestHelper.IntentRequest("TestIntent"));
        handlerInput.Configure().RequestEnvelope().Returns(intentRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Processing Alexa skill request").Once();
        logger.GetCapturedEntries().Where(e => e.Message.Contains("TestIntent")).Should().NotBeEmpty();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNonIntentRequest_LogsWithoutIntentName(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Shared] IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest launchRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(launchRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Processing Alexa skill request").Once();
        logger.GetCapturedEntries().Count(e => e.LogLevel == LogLevel.Debug).Should().Be(2);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_CallsNextDelegate_ExactlyOnce(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest skillRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(skillRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        next.CallCount.Should().Be(1);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_CreatesProperScope_WithRequestContext(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest skillRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(skillRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert - scope creation and proper logging should occur
        logger.GetCapturedEntries().Count(e => e.LogLevel == LogLevel.Debug).Should().Be(2);
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Processing Alexa skill request").Once();
        logger.Verify().AtLevel(LogLevel.Debug).WithMessageContaining("Successfully processed Alexa skill request").Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Record.Exception(() => new PerformanceLoggingBehavior(null!));

        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_LogsRequestTypeAndApplicationId(
        ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Shared] IHandlerInput handlerInput,
        [Shared] FakeRequestHandlerDelegate next,
        SkillResponse expectedResponse,
        [Shared] SkillRequest launchRequest)
    {
        // Arrange
        handlerInput.Configure().RequestEnvelope().Returns(launchRequest);
        next.Returns(Task.FromResult(expectedResponse));

        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);

        // Assert
        logger.GetCapturedEntries().Where(e => e.Message.Contains("LaunchRequest")).Should().NotBeEmpty();
        logger.GetCapturedEntries().Count(e => e.LogLevel == LogLevel.Debug).Should().Be(2);
    }
}
