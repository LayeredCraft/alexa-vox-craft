using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Lambda.Tests;

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
    [AutoData]
    public void CreateContext_WithValidRequest_CallsFactory(SkillRequest launchRequest)
    {
        var function = new TestAlexaSkillFunction();
        
        // This test verifies the context creation doesn't throw
        var exception = Record.Exception(() => function.CreateContext(launchRequest));
        
        exception.Should().BeNull();
    }

    [Theory]
    [AutoData]
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
    [AutoData]
    public async Task FunctionHandlerAsync_WithNullRequest_ThrowsException(ILambdaContext lambdaContext)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(null!, lambdaContext));

        exception.Should().NotBeNull();
    }

    [Theory]
    [AutoData]
    public async Task FunctionHandlerAsync_WithNullLambdaContext_ThrowsException(SkillRequest skillRequest)
    {
        var function = new TestAlexaSkillFunction();

        var exception = await Record.ExceptionAsync(() => 
            function.FunctionHandlerAsync(skillRequest, null!));

        exception.Should().NotBeNull();
    }

    [Theory]
    [AutoData]
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
    [AutoData]
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
    [AutoData]
    public void CreateContext_WithLaunchRequest_CreatesLaunchRequest(SkillRequest launchRequest)
    {
        // Verify our specimen builder creates a launch request based on parameter name
        launchRequest.Request.Type.Should().Be("LaunchRequest");
    }

    [Theory]
    [AutoData]
    public void CreateContext_WithIntentRequest_CreatesIntentRequest(SkillRequest intentRequest)
    {
        // Verify our specimen builder creates an intent request based on parameter name
        intentRequest.Request.Type.Should().Be("IntentRequest");
    }

    [Theory]
    [AutoData]
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
    [AutoData]
    public void CreateContext_WithSessionEndRequest_CreatesSessionEndedRequest(SkillRequest sessionEndRequest)
    {
        // Verify our specimen builder creates a session ended request based on parameter name
        sessionEndRequest.Request.Type.Should().Be("SessionEndedRequest");
    }

    [Theory]
    [AutoData]
    public void CreateContext_WithAudioPlayerRequest_CreatesAudioPlayerRequest(SkillRequest audioPlayerRequest)
    {
        // Verify our specimen builder creates an audio player request based on parameter name
        audioPlayerRequest.Request.Type.Should().Be("AudioPlayer.PlaybackStopped");
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