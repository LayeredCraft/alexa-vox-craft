using System.Text;
using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MinimalLambda;
using AlexaVoxCraft.MinimalLambda.Extensions;
using AlexaVoxCraft.NativeAot.TestFixture;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalLambda;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// Exercises the complete MinimalLambda integration boundary that a deployed Native AOT Lightsaber
/// Lambda exposed: consumer handler activation through <see cref="ServiceCollectionExtensions"/>,
/// generated mediator dispatch, and Lambda request/response serialization.
/// </summary>
public sealed class MinimalLambdaHostTests
{
    [Fact]
    public async Task AddAlexaSkillHost_ActivatesConstructorInjectedHandler_AndDispatchesLaunchRequest()
    {
        const string skillId = "amzn1.ask.skill.54b58c306f70c433";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Skill:SkillId"] = skillId })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<HostActivationProbe>();
        services.AddSkillMediator(configuration, builder =>
        {
            builder.SkillId = skillId;
            builder.RegisterServicesFromAssemblyContaining<LaunchHandler>();
        });
        services.AddAlexaSkillHost<TestHostHandler, SkillRequest, SkillResponse>();
        services.AddScoped<SkillRequestFactory>(_ => () => AmbientRequest.Current);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var serializer = scope.ServiceProvider.GetRequiredService<ILambdaSerializer>();
        using var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("LaunchRequest.json")));
        var request = serializer.Deserialize<SkillRequest>(requestStream);
        Assert.IsType<LaunchRequest>(request?.Request);

        AmbientRequest.Current = request;
        try
        {
            var handler = scope.ServiceProvider.GetRequiredService<HandlerDelegate<SkillRequest, SkillResponse>>();
            var response = await AlexaHandler.Invoke(request!, handler, NativeAotLambdaContext.Instance, CancellationToken.None);

            using var responseStream = new MemoryStream();
            serializer.Serialize(response, responseStream);

            Assert.True(scope.ServiceProvider.GetRequiredService<HostActivationProbe>().WasUsed);
            Assert.IsType<SsmlOutputSpeech>(response.Response?.OutputSpeech);
            Assert.True(responseStream.Length > 0);
        }
        finally
        {
            AmbientRequest.Current = null;
        }
    }
}

public sealed class NativeAotLambdaContext : ILambdaContext
{
    public static NativeAotLambdaContext Instance { get; } = new();
    public string AwsRequestId => "native-aot-host-boundary";
    public IClientContext ClientContext => null!;
    public string FunctionName => "NativeAotHostBoundary";
    public string FunctionVersion => "1";
    public ICognitoIdentity Identity => null!;
    public string InvokedFunctionArn => "arn:aws:lambda:us-east-1:123456789012:function:NativeAotHostBoundary";
    public ILambdaLogger Logger => NullLambdaLogger.Instance;
    public string LogGroupName => "native-aot-host-boundary";
    public string LogStreamName => "native-aot-host-boundary";
    public int MemoryLimitInMB => 512;
    public TimeSpan RemainingTime => TimeSpan.FromMinutes(1);
}

public sealed class NullLambdaLogger : ILambdaLogger
{
    public static NullLambdaLogger Instance { get; } = new();
    public void Log(string message) { }
    public void LogLine(string message) { }
}
