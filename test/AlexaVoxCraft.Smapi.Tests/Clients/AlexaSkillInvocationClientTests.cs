using System.Net;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using AlexaVoxCraft.Smapi.Tests.TestKit;
using Compono;
using Compono.Http;
using Compono.XunitV3;

namespace AlexaVoxCraft.Smapi.Tests.Clients;

// ADR-0002 Amendment 3/ADR-0052 (Part B) - see AlexaInteractionModelClientTests.cs's own
// class-level comment for the full reasoning; same fix, same shared SmapiHttpTestProfile.
public sealed class AlexaSkillInvocationClientTests
{
    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_RequestIsValid_ReturnsResponse(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.OnPost(Match.Any<string>()).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/{stage}/invocations";
        var registration = handler.OnPost(expectedUri).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_UsesPostMethod(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var registration = handler.When(req => req.Method == HttpMethod.Post).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_WhenNotFound_ReturnsNull(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest)
    {
        handler.OnPost(Match.Any<string>()).Respond(HttpStatusCode.NotFound);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_WithDefaultRegion_UsesDefaultEndpointRegion(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.OnPost(Match.Any<string>()).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, InvocationRegion.Default, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>(InvocationRegion.NA)]
    public async Task InvokeAsync_WithNaRegion_CompletesSuccessfully(
        InvocationRegion region,
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        await AssertRegionCompletesSuccessfully(region, handler, client, skillId, stage, skillRequest, responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>(InvocationRegion.EU)]
    public async Task InvokeAsync_WithEuRegion_CompletesSuccessfully(
        InvocationRegion region,
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        await AssertRegionCompletesSuccessfully(region, handler, client, skillId, stage, skillRequest, responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>(InvocationRegion.FE)]
    public async Task InvokeAsync_WithFeRegion_CompletesSuccessfully(
        InvocationRegion region,
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        await AssertRegionCompletesSuccessfully(region, handler, client, skillId, stage, skillRequest, responseModel);
    }

    private static async Task AssertRegionCompletesSuccessfully(
        InvocationRegion region, HttpTestHarness handler, AlexaSkillInvocationClient client, string skillId,
        string stage, SkillRequest skillRequest, SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.OnPost(Match.Any<string>()).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, region, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_WithDevelopmentStage_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/development/invocations";
        var registration = handler.OnPost(expectedUri).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, "development", skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_WithLiveStage_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/live/invocations";
        var registration = handler.OnPost(expectedUri).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, "live", skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task InvokeAsync_ResponseContainsStatus_ReturnsCorrectStatus(
        [Shared] HttpTestHarness handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.OnPost(Match.Any<string>()).RespondJson(responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result!.Status.Should().Be(responseModel.Status);
    }
}
