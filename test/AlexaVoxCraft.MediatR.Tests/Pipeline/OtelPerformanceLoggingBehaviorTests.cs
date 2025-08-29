using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AutoFixture.Xunit3;
using AwesomeAssertions;
using NSubstitute;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace AlexaVoxCraft.MediatR.Tests.Pipeline;

[Collection("DiagnosticsConfig Tests")]
public class OtelPerformanceLoggingBehaviorTests : TestBase
{
    private readonly ActivityListener _activityListener;
    private readonly List<Activity> _capturedActivities;
    private readonly MeterProvider _meterProvider;
    private readonly List<MetricSnapshot> _capturedMetrics;

    public OtelPerformanceLoggingBehaviorTests()
    {
        AlexaVoxCraftTelemetry.ResetForTesting();
        // Setup activity capturing
        _capturedActivities = [];
        _activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name.Contains(AlexaVoxCraftTelemetry.ActivitySourceName),
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => _capturedActivities.Add(activity),
        };
        ActivitySource.AddActivityListener(_activityListener);

        // Setup metrics capturing
        _capturedMetrics = [];
        _meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(AlexaVoxCraftTelemetry.MeterName) // Use the actual meter name from DiagnosticsConfig
            .AddInMemoryExporter(_capturedMetrics)
            .Build();

    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSuccessfulRequest_LogsDebugMessages(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        next.Invoke().Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.OperationName.Should().Be(AlexaSpanNames.Request);
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RpcSystem,
                AlexaSemanticValues.RpcSystemAlexa));
        activity.TagObjects.Should().ContainEquivalentOf(new KeyValuePair<string, object?>(
            AlexaSemanticAttributes.RpcService, handlerInput.RequestEnvelope.Context.System.Application.ApplicationId));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RpcMethod,
                handlerInput.RequestEnvelope.Request.Type));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RequestType,
                handlerInput.RequestEnvelope.Request.Type));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.Locale,
                handlerInput.RequestEnvelope.Request.Locale));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.SessionNew,
                AlexaSemanticValues.False));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.DeviceHasScreen,
                AlexaSemanticValues.False));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RequestId,
                handlerInput.RequestEnvelope.Request.RequestId));
        activity.Status.Should().Be(ActivityStatusCode.Ok);
        
        _capturedMetrics.Should().NotBeEmpty("Metrics should have been captured");
        var latency = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Latency)
            .ToList();
        latency.Should().NotBeEmpty("Latency histogram should be recorded");
        var requests = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Requests)
            .ToList();
        requests.Should().NotBeEmpty("Request counter should be recorded");

    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithArgumentException_RecordsValidationError(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");
        next.Invoke().Returns(Task.FromException<SkillResponse>(exception));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            behavior.Handle(handlerInput, CancellationToken, next));

        FlushMetrics();
        
        // Assert activity
        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.Status.Should().Be(ActivityStatusCode.Error);
        activity.StatusDescription.Should().Be(exception.Message);

        // Assert exception event was recorded
        var exceptionEvent = activity.Events.Should().ContainSingle(e => e.Name == AlexaEventNames.Exception).Subject;
        exceptionEvent.Tags.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.ExceptionType, typeof(ArgumentException).FullName!));
        exceptionEvent.Tags.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.ExceptionMessage, exception.Message));

        // Assert error metrics
        var errorMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Errors)
            .ToList();
        errorMetrics.Should().NotBeEmpty("Error counter should be recorded");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithInvalidOperationException_RecordsBusinessError(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var exception = new InvalidOperationException("Invalid operation");
        next.Invoke().Returns(Task.FromException<SkillResponse>(exception));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            behavior.Handle(handlerInput, CancellationToken, next));

        FlushMetrics();
        
        // Assert activity status
        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.Status.Should().Be(ActivityStatusCode.Error);

        // Assert error metrics with business error type
        var errorMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Errors)
            .ToList();
        errorMetrics.Should().NotBeEmpty("Error counter should be recorded for business errors");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithTimeoutException_RecordsTimeoutError(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var exception = new TimeoutException("Operation timed out");
        next.Invoke().Returns(Task.FromException<SkillResponse>(exception));

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() => 
            behavior.Handle(handlerInput, CancellationToken, next));

        FlushMetrics();
        
        // Assert activity
        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.Status.Should().Be(ActivityStatusCode.Error);

        // Assert error metrics
        var errorMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Errors)
            .ToList();
        errorMetrics.Should().NotBeEmpty("Error counter should be recorded for timeout errors");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithUnknownException_RecordsUnhandledError(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next)
    {
        // Arrange
        var exception = new NotImplementedException("Unknown error type");
        next.Invoke().Returns(Task.FromException<SkillResponse>(exception));

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            behavior.Handle(handlerInput, CancellationToken, next));

        FlushMetrics();
        
        // Assert activity
        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.Status.Should().Be(ActivityStatusCode.Error);

        // Assert error metrics
        var errorMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Errors)
            .ToList();
        errorMetrics.Should().NotBeEmpty("Error counter should be recorded for unhandled errors");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithColdStart_RecordsColdStartMetrics(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange
        AlexaVoxCraftTelemetry.ResetForTesting(); // Ensure this is a cold start
        next.Invoke().Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasColdStart,
                AlexaSemanticValues.True));

        // Assert cold start counter was recorded
        var coldStartMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.ColdStarts)
            .ToList();
        coldStartMetrics.Should().NotBeEmpty("Cold start counter should be recorded");

        // Assert request metrics include cold start tag
        var requestMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Requests)
            .ToList();
        requestMetrics.Should().NotBeEmpty("Request counter should be recorded");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithWarmStart_DoesNotRecordColdStartMetrics(
        PerformanceLoggingBehavior behavior,
        IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillResponse expectedResponse)
    {
        // Arrange - Simulate warm start by calling IsColdStart() first
        AlexaVoxCraftTelemetry.IsColdStart(); // This call makes subsequent calls return false
        next.Invoke().Returns(Task.FromResult(expectedResponse));

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .NotContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasColdStart,
                AlexaSemanticValues.True));

        // Assert cold start counter was NOT recorded
        var coldStartMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.ColdStarts)
            .ToList();
        coldStartMetrics.Should().BeEmpty("Cold start counter should not be recorded for warm starts");

        // Assert request metrics include warm start tag
        var requestMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Requests)
            .ToList();
        requestMetrics.Should().NotBeEmpty("Request counter should still be recorded");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithIntentRequest_RecordsIntentSpecificTelemetry(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest intentRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "intentRequest" will create IntentRequest
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(intentRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.IntentName, "TestIntent"));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RpcMethod, "TestIntent"));

        // Assert request metrics include intent name
        var requestMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.Requests)
            .ToList();
        requestMetrics.Should().NotBeEmpty("Request counter should be recorded for intent requests");
    }

    [Theory]
    [MediatRAutoData] 
    public async Task Handle_WithHelpIntentRequest_RecordsHelpIntentTelemetry(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest helpIntentRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "helpIntentHandlerInput" will create HelpIntent
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(helpIntentRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.IntentName, "AMAZON.HelpIntent"));
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RpcMethod, "AMAZON.HelpIntent"));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSessionEndedRequest_RecordsSessionEndTelemetry(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest sessionEndRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "sessionEndRequest" will create SessionEndedRequest
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(sessionEndRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RequestType, "SessionEndedRequest"));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithAudioPlayerRequest_RecordsAudioTelemetry(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest audioRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "audioRequest" will create AudioPlayerRequest
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(audioRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.RequestType, "AudioPlayer.PlaybackStopped"));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithDisplayRequest_RecordsScreenDeviceCapability(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest displayRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "displayRequest" will create DisplayElementSelectedRequest
        // Mock device with screen capability by adding Alexa.Presentation.APL interface
        var supportedInterfaces = new Dictionary<string, object>
        {
            { "Alexa.Presentation.APL", new { } }
        };
        
        // Ensure Device object exists and has SupportedInterfaces
        if (displayRequest.Context.System.Device == null)
        {
            displayRequest.Context.System.Device = new Device();
        }
        displayRequest.Context.System.Device.SupportedInterfaces = supportedInterfaces;
        
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(displayRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.DeviceHasScreen, AlexaSemanticValues.True));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithLaunchRequest_RecordsNonScreenDeviceCapability(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest launchRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - parameter named "launchRequest" will create LaunchRequest
        // Ensure device has no screen interfaces (default state)
        if (launchRequest.Context.System.Device == null)
        {
            launchRequest.Context.System.Device = new Device();
        }
        launchRequest.Context.System.Device.SupportedInterfaces = new Dictionary<string, object>();
        
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(launchRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.DeviceHasScreen, AlexaSemanticValues.False));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNewSession_RecordsNewSessionAttribute(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest intentRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - Set session as new
        intentRequest.Session.New = true;
        
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(intentRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.SessionNew, AlexaSemanticValues.True));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithExistingSession_RecordsExistingSessionAttribute(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest intentRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - Set session as existing (not new)
        intentRequest.Session.New = false;
        
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(intentRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.SessionNew, AlexaSemanticValues.False));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithSessionlessRequest_DoesNotRecordSessionAttribute(
        PerformanceLoggingBehavior behavior,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] RequestHandlerDelegate next,
        SkillRequest audioRequest,
        SkillResponse expectedResponse)
    {
        // Arrange - AudioPlayerRequest typically doesn't have a session
        audioRequest.Session = null!;
        
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        handlerInput.RequestEnvelope.Returns(audioRequest);

        // Act
        var result = await behavior.Handle(handlerInput, CancellationToken, next);

        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);

        var activity = _capturedActivities.Should().ContainSingle().Subject;
        activity.TagObjects.Should()
            .ContainEquivalentOf(new KeyValuePair<string, object?>(AlexaSemanticAttributes.SessionNew, AlexaSemanticValues.False));
    }
    
    /// <summary>
    /// Forces collection of metrics by triggering the meter provider to export.
    /// Call this before asserting on captured metrics.
    /// </summary>
    private void FlushMetrics()
    {
        _meterProvider.ForceFlush(1000); // 1000ms timeout
    }

}