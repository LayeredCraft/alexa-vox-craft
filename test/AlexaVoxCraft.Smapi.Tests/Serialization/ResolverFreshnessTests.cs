using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using Microsoft.Extensions.Logging.Abstractions;

namespace AlexaVoxCraft.Smapi.Tests.Serialization;

/// <summary>
/// AlexaJsonOptions is process-global mutable state - see AlexaVoxCraft.Model.Tests'
/// AlexaJsonOptionsTests.cs for the same rationale. These tests must not run in parallel with each other.
/// </summary>
[CollectionDefinition(nameof(SmapiResolverFreshnessCollection), DisableParallelization = true)]
public sealed class SmapiResolverFreshnessCollection;

[Collection(nameof(SmapiResolverFreshnessCollection))]
public sealed class ResolverFreshnessTests
{
    [Fact]
    public void InteractionModelBuilder_ToJson_WorksStandalone_NoDiContainer()
    {
        var builder = InteractionModelBuilder.Create()
            .WithInvocationName("test invocation")
            .WithVersion("1.0")
            .WithDescription("test skill");

        var json = builder.ToJson();

        json.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AlexaSkillInvocationClient_ConstructedBeforeRegistration_StillResolvesConsumerType_AfterRegistration()
    {
        AlexaJsonOptions.RegisterTypeInfoResolver(FreshnessConsumerContext.Default);

        var handler = new StubHttpMessageHandler((_, _) =>
        {
            var body = JsonSerializer.Serialize(new SkillInvocationResponse<FreshnessConsumerPoco>
            {
                Status = "SUCCESSFUL",
                Result = new SkillInvocationResult<FreshnessConsumerPoco>
                {
                    SkillExecutionInfo = new SkillExecutionInfo<FreshnessConsumerPoco>
                    {
                        InvocationResponse = new InvocationResponseInfo<FreshnessConsumerPoco>
                        {
                            Body = new FreshnessConsumerPoco { Value = "resolved-after-construction" }
                        }
                    }
                }
            }, AlexaJsonOptions.DefaultOptions);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };
        var client = new AlexaSkillInvocationClient(httpClient, NullLogger<AlexaSkillInvocationClient>.Instance);

        var result = await client.InvokeAsync<FreshnessConsumerPoco, FreshnessConsumerPoco>(
            "skill-id", "development", new FreshnessConsumerPoco { Value = "request" },
            ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Result!.SkillExecutionInfo!.InvocationResponse!.Body!.Value.Should().Be("resolved-after-construction");
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(responder(request, cancellationToken));
    }
}

public sealed class FreshnessConsumerPoco
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(FreshnessConsumerPoco))]
[JsonSerializable(typeof(SkillInvocationResponse<FreshnessConsumerPoco>))]
[JsonSerializable(typeof(SkillInvocationRequest<FreshnessConsumerPoco>))]
[JsonSerializable(typeof(SkillInvocationBody<FreshnessConsumerPoco>))]
[JsonSerializable(typeof(SkillInvocationResult<FreshnessConsumerPoco>))]
[JsonSerializable(typeof(SkillExecutionInfo<FreshnessConsumerPoco>))]
[JsonSerializable(typeof(InvocationResponseInfo<FreshnessConsumerPoco>))]
internal partial class FreshnessConsumerContext : JsonSerializerContext;
