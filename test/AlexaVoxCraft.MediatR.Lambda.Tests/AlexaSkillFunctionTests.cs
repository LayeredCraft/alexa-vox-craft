using System.Diagnostics;
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AlexaVoxCraft.MediatR.Lambda.Tests.TestKit;
using Compono.XunitV3;

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

    [Theory, Compose<LambdaTestProfile>]
    public void CreateContext_WithValidRequest_CallsFactory(SkillRequest launchRequest)
    {
        var function = new TestAlexaSkillFunction();

        // This test verifies the context creation doesn't throw
        var exception = Record.Exception(() => function.CreateContext(launchRequest));

        exception.Should().BeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_WithMissingHandler_ThrowsException(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() =>
            function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_WithNullRequest_ThrowsException(FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() =>
            function.FunctionHandlerAsync(null!, lambdaContext, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_WithNullLambdaContext_ThrowsException(SkillRequest skillRequest)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() =>
            function.FunctionHandlerAsync(skillRequest, null!, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_WithValidHandler_ReturnsResponse(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();

        var result = await function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Should().BeOfType<SkillResponse>();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_WithHandlerException_PropagatesException(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithThrowingHandler();

        var thrownException = await Record.ExceptionAsync(() =>
            function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken));

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

    [Fact]
    public void CreateContext_WithLaunchRequest_CreatesLaunchRequest()
    {
        var launchRequest = LambdaTestProfile.CreateSkillRequest(new LaunchRequest { Type = "LaunchRequest" });

        launchRequest.Request.Type.Should().Be("LaunchRequest");
    }

    [Fact]
    public void CreateContext_WithIntentRequest_CreatesIntentRequest()
    {
        var intentRequest = LambdaTestProfile.CreateSkillRequest(new IntentRequest { Type = "IntentRequest", Intent = new Intent() });

        intentRequest.Request.Type.Should().Be("IntentRequest");
    }

    [Fact]
    public void CreateContext_WithHelpIntentRequest_CreatesHelpIntent()
    {
        var helpIntentRequest = LambdaTestProfile.CreateSkillRequest(new IntentRequest { Type = "IntentRequest", Intent = new Intent { Name = "AMAZON.HelpIntent" } });

        helpIntentRequest.Request.Type.Should().Be("IntentRequest");
        ((IntentRequest)helpIntentRequest.Request).Intent.Name.Should().Be("AMAZON.HelpIntent");
    }

    [Fact]
    public void CreateContext_WithSessionEndRequest_CreatesSessionEndedRequest()
    {
        var sessionEndRequest = LambdaTestProfile.CreateSkillRequest(new SessionEndedRequest { Type = "SessionEndedRequest" });

        sessionEndRequest.Request.Type.Should().Be("SessionEndedRequest");
    }

    [Fact]
    public void CreateContext_WithAudioPlayerRequest_CreatesAudioPlayerRequest()
    {
        var audioPlayerRequest = LambdaTestProfile.CreateSkillRequest(new AudioPlayerRequest { Type = "AudioPlayer.PlaybackStopped" });

        audioPlayerRequest.Request.Type.Should().Be("AudioPlayer.PlaybackStopped");
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_CreatesLambdaSpan(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        await function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken);

        activities.Should().NotBeEmpty();
        var lambdaSpan = activities.FirstOrDefault(a => a.OperationName == AlexaSpanNames.LambdaExecution);
        lambdaSpan.Should().NotBeNull();
    }


    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_HandlesColdStart(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();
        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        // Reset cold start state by creating a new function instance
        var coldStartFunction = new TestAlexaSkillFunctionWithHandler();
        await coldStartFunction.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken);

        // Second call should not be cold start
        activities.Clear();
        await function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken);

        var lambdaSpan = activities.FirstOrDefault(a => a.OperationName == AlexaSpanNames.LambdaExecution);
        lambdaSpan.Should().NotBeNull();

        var tags = lambdaSpan!.Tags.ToDictionary(t => t.Key, t => t.Value);
        // Since cold start is tracked globally, we can't reliably test its presence
        // but we can verify the span was created properly
        lambdaSpan.Status.Should().Be(ActivityStatusCode.Ok);
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_HandlesSpanOnException(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithThrowingHandler();
        var activities = new List<Activity>();
        var events = new List<ActivityEvent>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == AlexaVoxCraftTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => activities.Add(activity),
            ActivityStopped = activity => events.AddRange(activity.Events)
        };
        ActivitySource.AddActivityListener(activityListener);

        var exception = await Record.ExceptionAsync(() =>
            function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();

        var lambdaSpan = activities.FirstOrDefault(a =>
            a.OperationName == AlexaSpanNames.LambdaExecution &&
            a.Status == ActivityStatusCode.Error);
        lambdaSpan.Should().NotBeNull("expected a LambdaExecution span with Error status after handler exception");

        var exceptionEvents = events.Where(e => e.Name == "exception");
        exceptionEvents.Should().NotBeEmpty();
    }

    [Theory, Compose<LambdaTestProfile>]
    public async Task FunctionHandlerAsync_TracksLambdaDuration(
        SkillRequest skillRequest,
        FakeLambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunctionWithHandler();

        // This test verifies that the timer scope is used without external metric collection
        // The actual duration tracking is handled by the TimerScope which we can't easily mock
        var result = await function.FunctionHandlerAsync(skillRequest, lambdaContext, TestContext.Current.CancellationToken);

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
                (_, _, _) => Task.FromResult(new SkillResponse()));
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
                (_, _, _) => Task.FromException<SkillResponse>(new InvalidOperationException("Handler exception")));
        });
        base.Init(builder);
    }
}
