using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.MediatR.Wrappers;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using AutoFixture.Xunit3;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace AlexaVoxCraft.MediatR.Tests.Wrappers;

[Collection("DiagnosticsConfig Tests")]
public class OtelRequestHandlerWrapperTests : TestBase
{
    private readonly ActivityListener _activityListener;
    private readonly List<Activity> _capturedActivities;
    private readonly MeterProvider _meterProvider;
    private readonly List<MetricSnapshot> _capturedMetrics;

    public OtelRequestHandlerWrapperTests()
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
            .AddMeter(AlexaVoxCraftTelemetry.MeterName)
            .AddInMemoryExporter(_capturedMetrics)
            .Build();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMatchingHandler_RecordsHandlerResolutionAndExecutionTelemetry(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        handler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);
        
        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);
        
        // Verify activities were created
        _capturedActivities.Should().HaveCount(2);
        var resolutionActivity = _capturedActivities.Should().ContainSingle(a => a.OperationName == AlexaSpanNames.HandlerResolution).Subject;
        var executionActivity = _capturedActivities.Should().ContainSingle(a => a.OperationName == AlexaSpanNames.Handler).Subject;
        
        // Verify resolution activity
        resolutionActivity.Status.Should().Be(ActivityStatusCode.Ok);
        resolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, handler.GetType().Name));
        resolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, 0));
        resolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, false));
        resolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, true));
        
        // Verify execution activity
        executionActivity.Status.Should().Be(ActivityStatusCode.Ok);
        executionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, handler.GetType().Name));
        executionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, false));
        
        // Verify metrics
        var resolutionMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerResolutionAttempts)
            .ToList();
        resolutionMetrics.Should().NotBeEmpty("Handler resolution attempt counter should be recorded");
        
        var executionMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerExecutions)
            .ToList();
        executionMetrics.Should().NotBeEmpty("Handler execution counter should be recorded");
        
        var durationMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerDuration)
            .ToList();
        durationMetrics.Should().NotBeEmpty("Handler duration histogram should be recorded");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNonMatchingHandler_RecordsResolutionWithoutExecution(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        defaultHandler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        defaultHandler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(defaultHandler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);
        
        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);
        
        // Should have 3 activities: non-matching handler resolution, default handler resolution, default handler execution
        _capturedActivities.Should().HaveCount(3);
        
        var regularResolutionActivity = _capturedActivities
            .Where(a => a.OperationName == AlexaSpanNames.HandlerResolution && 
                       a.TagObjects.Any(t => t.Key == AlexaSemanticAttributes.HandlerIsDefault && 
                                           t.Value?.ToString() == "False"))
            .Should().ContainSingle().Subject;
        
        var defaultResolutionActivity = _capturedActivities
            .Where(a => a.OperationName == AlexaSpanNames.HandlerResolution && 
                       a.TagObjects.Any(t => t.Key == AlexaSemanticAttributes.HandlerIsDefault && 
                                           t.Value?.ToString() == "True"))
            .Should().ContainSingle().Subject;
        
        var defaultExecutionActivity = _capturedActivities.Should()
            .ContainSingle(a => a.OperationName == AlexaSpanNames.Handler).Subject;
        
        // Verify regular handler resolution (failed)
        regularResolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, false));
        regularResolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, 0));
        
        // Verify default handler resolution (succeeded)
        defaultResolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, true));
        defaultResolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, 1));
        defaultResolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, true));
        
        // Verify default handler execution
        defaultExecutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, true));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithMultipleHandlers_RecordsCorrectExecutionOrder(
        SkillRequest skillRequest,
        SkillResponse expectedResponse,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler1,
        IRequestHandler<LaunchRequest> handler2,
        IRequestHandler<LaunchRequest> handler3)
    {
        // Arrange
        handler1.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        handler2.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        handler3.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        handler3.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(expectedResponse);
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler1);
        services.AddSingleton(handler2);
        services.AddSingleton(handler3);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act
        var result = await wrapper.Handle(skillRequest, serviceProvider, CancellationToken);
        
        FlushMetrics();
        
        // Assert
        result.Should().Be(expectedResponse);
        
        // Should have 4 activities: 3 resolution attempts + 1 execution
        _capturedActivities.Should().HaveCount(4);
        
        var resolutionActivities = _capturedActivities
            .Where(a => a.OperationName == AlexaSpanNames.HandlerResolution)
            .OrderBy(a => a.TagObjects.First(t => t.Key == AlexaSemanticAttributes.HandlerExecutionOrder).Value)
            .ToList();
        
        resolutionActivities.Should().HaveCount(3);
        
        // Verify execution order is correct
        for (int i = 0; i < resolutionActivities.Count; i++)
        {
            resolutionActivities[i].TagObjects.Should().ContainEquivalentOf(
                new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, i));
        }
        
        // First two handlers should not be able to handle
        resolutionActivities[0].TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, false));
        resolutionActivities[1].TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, false));
        
        // Third handler should be able to handle
        resolutionActivities[2].TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, true));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithHandlerResolutionException_RecordsErrorTelemetry(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        var exception = new InvalidOperationException("Handler resolution failed");
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(Task.FromException<bool>(exception));
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));
        
        FlushMetrics();
        
        // Verify error telemetry
        var resolutionActivity = _capturedActivities.Should()
            .ContainSingle(a => a.OperationName == AlexaSpanNames.HandlerResolution).Subject;
        
        resolutionActivity.Status.Should().Be(ActivityStatusCode.Error);
        resolutionActivity.StatusDescription.Should().Be(exception.Message);
        
        // Handler resolution attempt should still be recorded
        var resolutionMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerResolutionAttempts)
            .ToList();
        resolutionMetrics.Should().NotBeEmpty("Handler resolution attempt should still be recorded even on error");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithHandlerExecutionException_RecordsErrorTelemetry(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler)
    {
        // Arrange
        var exception = new ArgumentException("Handler execution failed");
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        handler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(Task.FromException<SkillResponse>(exception));
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));
        
        FlushMetrics();
        
        // Verify both resolution and execution activities
        _capturedActivities.Should().HaveCount(2);
        
        var resolutionActivity = _capturedActivities.Should()
            .ContainSingle(a => a.OperationName == AlexaSpanNames.HandlerResolution).Subject;
        var executionActivity = _capturedActivities.Should()
            .ContainSingle(a => a.OperationName == AlexaSpanNames.Handler).Subject;
        
        // Resolution should be OK
        resolutionActivity.Status.Should().Be(ActivityStatusCode.Ok);
        resolutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerCanHandle, true));
        
        // Execution should be Error
        executionActivity.Status.Should().Be(ActivityStatusCode.Error);
        executionActivity.StatusDescription.Should().Be(exception.Message);
        
        // Both resolution attempt and execution should be recorded in metrics
        var resolutionMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerResolutionAttempts)
            .ToList();
        resolutionMetrics.Should().NotBeEmpty();
        
        var executionMetrics = _capturedMetrics
            .Where(m => m.Name == AlexaMetricNames.HandlerExecutions)
            .ToList();
        executionMetrics.Should().NotBeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithDefaultHandlerException_RecordsDefaultHandlerErrorTelemetry(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services,
        IRequestHandler<LaunchRequest> handler,
        IDefaultRequestHandler defaultHandler)
    {
        // Arrange
        var exception = new TimeoutException("Default handler execution failed");
        handler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(false);
        defaultHandler.CanHandle(handlerInput, Arg.Any<CancellationToken>()).Returns(true);
        defaultHandler.Handle(handlerInput, Arg.Any<CancellationToken>()).Returns(Task.FromException<SkillResponse>(exception));
        
        services.AddSingleton(handlerInput);
        services.AddSingleton(handler);
        services.AddSingleton(defaultHandler);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));
        
        FlushMetrics();
        
        // Should have 3 activities: regular handler resolution, default handler resolution, default handler execution
        _capturedActivities.Should().HaveCount(3);
        
        var defaultExecutionActivity = _capturedActivities.Should()
            .ContainSingle(a => a.OperationName == AlexaSpanNames.Handler).Subject;
        
        // Default handler execution should show error
        defaultExecutionActivity.Status.Should().Be(ActivityStatusCode.Error);
        defaultExecutionActivity.StatusDescription.Should().Be(exception.Message);
        defaultExecutionActivity.TagObjects.Should().ContainEquivalentOf(
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, true));
    }

    [Theory]
    [MediatRAutoData]
    public async Task Handle_WithNoHandlers_RecordsNoHandlerTelemetryAndThrows(
        SkillRequest skillRequest,
        [Frozen] IHandlerInput handlerInput,
        [Frozen] IServiceCollection services)
    {
        // Arrange - No handlers registered
        services.AddSingleton(handlerInput);
        
        var serviceProvider = services.BuildServiceProvider();
        var wrapper = new RequestHandlerWrapperImpl<LaunchRequest>();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            wrapper.Handle(skillRequest, serviceProvider, CancellationToken));
        
        FlushMetrics();
        
        // No activities should be recorded since no handlers were found
        _capturedActivities.Should().BeEmpty();
        
        // No metrics should be recorded
        _capturedMetrics.Should().BeEmpty();
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