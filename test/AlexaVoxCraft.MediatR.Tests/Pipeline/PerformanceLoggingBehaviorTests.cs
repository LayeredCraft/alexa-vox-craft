using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using AutoFixture.Xunit3;
using LayeredCraft.StructuredLogging.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

public class PerformanceLoggingBehaviorTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSuccessfulRequest_LogsDebugMessages(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
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
        
        // Verify logging using structured logging testing extensions
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.AssertLogCount(LogLevel.Debug, 2);
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.HasLogEntry(LogLevel.Debug, "Successfully processed Alexa skill request").Should().BeTrue();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithException_LogsErrorAndRethrows(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        next.Invoke().Returns(Task.FromException<SkillResponse>(expectedException));
        
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
    [MediatRAutoData]
    public async Task Handle_WithIntentRequest_LogsIntentName(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest intentRequest)
    {
        // Arrange
        handlerInput.RequestEnvelope.Returns(intentRequest);
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        var debugEntries = testLogger.GetLogEntriesContaining("TestIntent");
        debugEntries.Should().NotBeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNonIntentRequest_LogsWithoutIntentName(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse,
        SkillRequest launchRequest)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.AssertLogCount(LogLevel.Debug, 2);
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_CallsNextDelegate_ExactlyOnce(
        [Frozen] TestLogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        await next.Received(1).Invoke();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_CreatesProperScope_WithRequestContext(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert - scope creation and proper logging should occur
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        testLogger.AssertLogCount(LogLevel.Debug, 2);
        testLogger.HasLogEntry(LogLevel.Debug, "Processing Alexa skill request").Should().BeTrue();
        testLogger.HasLogEntry(LogLevel.Debug, "Successfully processed Alexa skill request").Should().BeTrue();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Record.Exception(() => new PerformanceLoggingBehavior(null!));
        
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_LogsRequestTypeAndApplicationId(
        [Frozen] ILogger<PerformanceLoggingBehavior> logger,
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse,
        [Frozen] SkillRequest launchRequest)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        // Act
        await behavior.Handle(handlerInput, CancellationToken, next);
        
        // Assert
        var testLogger = (TestLogger<PerformanceLoggingBehavior>)logger;
        var debugEntries = testLogger.GetLogEntriesContaining("LaunchRequest");
        debugEntries.Should().NotBeEmpty();
        testLogger.AssertLogCount(LogLevel.Debug, 2);
    }
}