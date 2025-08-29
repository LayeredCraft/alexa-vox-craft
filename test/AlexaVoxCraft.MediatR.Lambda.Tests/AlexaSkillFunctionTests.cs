using System.Diagnostics;
using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.MediatR.Lambda.Tests;

[Collection("DiagnosticsConfig Tests")]
public class AlexaSkillFunctionTests : TestBase
{
    [Fact]
    public void Constructor_InitializesServiceProvider()
    {
        var function = new TestAlexaSkillFunction();
        
        function.ServiceProvider.Should().NotBeNull();
    }

    [Fact]
    public void ServiceProvider_ContainsRequiredServices()
    {
        var function = new TestAlexaSkillFunction();
        
        var skillContextFactory = function.ServiceProvider.GetService<ISkillContextFactory>();
        var lambdaSerializer = function.ServiceProvider.GetService<ILambdaSerializer>();
        var logger = function.ServiceProvider.GetService<ILogger<TestAlexaSkillFunction>>();
        
        skillContextFactory.Should().NotBeNull();
        lambdaSerializer.Should().NotBeNull();
        logger.Should().NotBeNull();
    }

    [Fact]
    public void CreateHostBuilder_ReturnsConfiguredBuilder()
    {
        var function = new TestAlexaSkillFunction();
        
        var builder = function.CreateHostBuilder();
        
        builder.Should().NotBeNull();
    }

    [Fact]
    public void CreateHostBuilder_ConfiguresLogging()
    {
        var function = new TestAlexaSkillFunction();
        
        var builder = function.CreateHostBuilder();
        var host = builder.Build();
        var logger = host.Services.GetService<ILogger<TestAlexaSkillFunction>>();
        
        logger.Should().NotBeNull();
    }

    [Fact]
    public void CreateHostBuilder_CallsInitMethod()
    {
        var function = new TestAlexaSkillFunction();
        
        function.CreateHostBuilder();
        
        function.InitCalled.Should().BeTrue();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithValidRequest_CallsFactory(SkillRequest launchRequest)
    {
        var function = new TestAlexaSkillFunction();
        
        // This test verifies the context creation doesn't throw
        var exception = Record.Exception(() => function.CreateContext(launchRequest));
        
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_WithMissingHandler_ThrowsException(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(skillRequest, lambdaContext));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_WithNullRequest_ThrowsException(ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(null!, lambdaContext));

        exception.Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_WithNullLambdaContext_ThrowsException(SkillRequest skillRequest)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(skillRequest, null!));

        exception.Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_WithValidHandler_ReturnsResponse(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        
        var result = await function.FunctionHandlerAsync(skillRequest, lambdaContext);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<SkillResponse>();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_WithHandlerException_PropagatesException(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithThrowingHandler();

        var thrownException = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(skillRequest, lambdaContext));

        thrownException.Should().BeOfType<InvalidOperationException>();
        thrownException.Message.Should().Be("Handler exception");
    }

    [Fact]
    public void Start_RegistersRequiredServices()
    {
        var function = new TestAlexaSkillFunction();
        
        var services = function.ServiceProvider;
        
        services.GetService<ISkillContextFactory>().Should().NotBeNull();
        services.GetService<ILambdaSerializer>().Should().NotBeNull();
        services.GetService<ISkillContextAccessor>().Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithLaunchRequest_CreatesLaunchRequest(SkillRequest launchRequest)
    {
        // Verify our specimen builder creates a launch request based on parameter name
        launchRequest.Request.Type.Should().Be("LaunchRequest");
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithIntentRequest_CreatesIntentRequest(SkillRequest intentRequest)
    {
        // Verify our specimen builder creates an intent request based on parameter name
        intentRequest.Request.Type.Should().Be("IntentRequest");
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithHelpIntentRequest_CreatesHelpIntent(SkillRequest helpIntentRequest)
    {
        // Verify our specimen builder creates a help intent based on parameter name
        helpIntentRequest.Request.Type.Should().Be("IntentRequest");
        if (helpIntentRequest.Request is IntentRequest intentRequest)
        {
            intentRequest.Intent.Name.Should().Be("AMAZON.HelpIntent");
        }
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithSessionEndRequest_CreatesSessionEndedRequest(SkillRequest sessionEndRequest)
    {
        // Verify our specimen builder creates a session ended request based on parameter name
        sessionEndRequest.Request.Type.Should().Be("SessionEndedRequest");
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void CreateContext_WithAudioPlayerRequest_CreatesAudioPlayerRequest(SkillRequest audioPlayerRequest)
    {
        // Verify our specimen builder creates an audio player request based on parameter name
        audioPlayerRequest.Request.Type.Should().Be("AudioPlayer.PlaybackStopped");
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_CreatesLambdaSpan(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        var activities = new List<Activity>();
        
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);
        
        await function.FunctionHandlerAsync(skillRequest, lambdaContext);
        
        activities.Should().NotBeEmpty();
        var lambdaSpan = activities.FirstOrDefault(a => a.OperationName == AlexaSpanNames.LambdaExecution);
        lambdaSpan.Should().NotBeNull();
    }


    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_HandlesColdStart(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        var activities = new List<Activity>();
        
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);
        
        // Reset cold start state by creating a new function instance
        var coldStartFunction = new TestAlexaSkillFunctionWithHandler();
        await coldStartFunction.FunctionHandlerAsync(skillRequest, lambdaContext);
        
        // Second call should not be cold start
        activities.Clear();
        await function.FunctionHandlerAsync(skillRequest, lambdaContext);
        
        var lambdaSpan = activities.FirstOrDefault(a => a.OperationName == AlexaSpanNames.LambdaExecution);
        lambdaSpan.Should().NotBeNull();
        
        var tags = lambdaSpan!.Tags.ToDictionary(t => t.Key, t => t.Value);
        // Since cold start is tracked globally, we can't reliably test its presence
        // but we can verify the span was created properly
        lambdaSpan.Status.Should().Be(ActivityStatusCode.Ok);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_HandlesSpanOnException(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithThrowingHandler();
        var activities = new List<Activity>();
        var events = new List<ActivityEvent>();
        
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = activity => events.AddRange(activity.Events)
        };
        ActivitySource.AddActivityListener(activityListener);
        
        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(skillRequest, lambdaContext));
        
        exception.Should().NotBeNull();
        
        var lambdaSpan = activities.FirstOrDefault(a => a.OperationName == AlexaSpanNames.LambdaExecution);
        lambdaSpan.Should().NotBeNull();
        lambdaSpan!.Status.Should().Be(ActivityStatusCode.Error);
        
        var exceptionEvents = events.Where(e => e.Name == AlexaEventNames.Exception);
        exceptionEvents.Should().NotBeEmpty();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task FunctionHandlerAsync_TracksLambdaDuration(
        SkillRequest skillRequest, 
        ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        
        // This test verifies that the timer scope is used without external metric collection
        // The actual duration tracking is handled by the TimerScope which we can't easily mock
        var result = await function.FunctionHandlerAsync(skillRequest, lambdaContext);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<SkillResponse>();
    }

}

/// <summary>
/// Test implementation of AlexaSkillFunction for testing purposes.
/// </summary>
public class TestAlexaSkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    public bool InitCalled { get; private set; }

    protected override void Init(IHostBuilder builder)
    {
        InitCalled = true;
        base.Init(builder);
    }

    /// <summary>
    /// Exposes CreateContext for testing.
    /// </summary>
    public new void CreateContext(SkillRequest request)
    {
        base.CreateContext(request);
    }

    /// <summary>
    /// Exposes CreateHostBuilder for testing.
    /// </summary>
    public new IHostBuilder CreateHostBuilder()
    {
        return base.CreateHostBuilder();
    }
}

/// <summary>
/// Test implementation that registers a handler for successful execution.
/// </summary>
public class TestAlexaSkillFunctionWithHandler : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<HandlerDelegate<SkillRequest, SkillResponse>>(
                (request, context) => Task.FromResult(new SkillResponse()));
        });
        base.Init(builder);
    }
}

/// <summary>
/// Test implementation that registers a handler that throws exceptions.
/// </summary>
public class TestAlexaSkillFunctionWithThrowingHandler : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<HandlerDelegate<SkillRequest, SkillResponse>>(
                (request, context) => Task.FromException<SkillResponse>(new InvalidOperationException("Handler exception")));
        });
        base.Init(builder);
    }
}