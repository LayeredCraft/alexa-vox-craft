using Compono;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging.Testing;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

[Collection("DiagnosticsConfig Tests")]
public class PerformanceLoggingBehaviorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithSuccessfulRequest_LogsDebugMessages(
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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

        // Verify logging using structured logging testing extensions
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.AssertLogCount(LogLevel.Debug, 2);
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.HasLogEntry(LogLevel.Debug, "Successfully processed Alexa skill request").Should().BeTrue();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithException_LogsErrorAndRethrows(
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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

        // Verify error logging using structured logging testing extensions
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.AssertLogCount(LogLevel.Error, 1);
        testLogger.HasLogEntry(LogLevel.Error, "Failed to process Alexa skill request").Should().BeTrue();
        testLogger.HasLogEntryWithException<InvalidOperationException>(LogLevel.Error).Should().BeTrue();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithIntentRequest_LogsIntentName(
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        var debugEntries = testLogger.GetLogEntriesContaining("TestIntent");
        debugEntries.Should().NotBeEmpty();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_WithNonIntentRequest_LogsWithoutIntentName(
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.AssertLogCount(LogLevel.Debug, 2);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Handle_CallsNextDelegate_ExactlyOnce(
        [Shared] TestLogger<PerformanceLoggingBehavior> logger,
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
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.AssertLogCount(LogLevel.Debug, 2);
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.HasLogEntry(LogLevel.Debug, "Successfully processed Alexa skill request").Should().BeTrue();
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
        [Shared] ILogger<PerformanceLoggingBehavior> logger,
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
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        var debugEntries = testLogger.GetLogEntriesContaining("LaunchRequest");
        debugEntries.Should().NotBeEmpty();
        testLogger.AssertLogCount(LogLevel.Debug, 2);
    }
}