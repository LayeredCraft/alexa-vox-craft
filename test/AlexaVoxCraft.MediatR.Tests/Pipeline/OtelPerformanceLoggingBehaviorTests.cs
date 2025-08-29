using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.MediatR.Pipeline;
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
    
    /// <summary>
    /// Forces collection of metrics by triggering the meter provider to export.
    /// Call this before asserting on captured metrics.
    /// </summary>
    private void FlushMetrics()
    {
        _meterProvider.ForceFlush(1000); // 1000ms timeout
    }

}