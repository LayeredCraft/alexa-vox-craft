using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using Compono.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Scenario 7 (original console app): SMAPI - standalone InteractionModelBuilder.ToJson() (no DI), and
// AlexaSkillInvocationClient.InvokeAsync with a consumer-owned type, against Compono.Http's
// TestHttpHandler (replacing the original's hand-rolled FakeHttpMessageHandler).
public sealed class SmapiTests
{
    [Fact]
    public void InteractionModelBuilder_ToJson_WorksStandalone_NoDiContainer()
    {
        var builder = InteractionModelBuilder.Create()
            .WithInvocationName("validation skill")
            .WithVersion("1.0")
            .WithDescription("native aot test project");

        var json = builder.ToJson();

        Assert.False(string.IsNullOrWhiteSpace(json));
    }

    [Fact]
    public async Task AlexaSkillInvocationClient_InvokeAsync_WithConsumerOwnedType()
    {
        var responseBody = JsonSerializer.Serialize(new SkillInvocationResponse<GameState>
        {
            Status = "SUCCESSFUL",
            Result = new SkillInvocationResult<GameState>
            {
                SkillExecutionInfo = new SkillExecutionInfo<GameState>
                {
                    InvocationResponse = new InvocationResponseInfo<GameState> { Body = new GameState { Score = 7, Level = "smapi" } }
                }
            }
        }, AlexaJsonTypeInfo.For<SkillInvocationResponse<GameState>>());

        var handler = new TestHttpHandler();
        handler.OnPost("/v2/skills/skill-id/stages/development/invocations")
            .RespondText(responseBody, "application/json");

        var invocationClient = new AlexaSkillInvocationClient(
            new HttpClient(handler) { BaseAddress = new Uri("https://api.amazonalexa.com/") },
            NullLogger<AlexaSkillInvocationClient>.Instance);

        var invocationResult = await invocationClient.InvokeAsync<GameState, GameState>(
            "skill-id", "development", new GameState { Score = 1, Level = "start" }, ct: CancellationToken.None);

        var body = invocationResult?.Result?.SkillExecutionInfo?.InvocationResponse?.Body;
        Assert.NotNull(body);
        Assert.Equal(7, body.Score);
        Assert.Equal("smapi", body.Level);
    }
}
