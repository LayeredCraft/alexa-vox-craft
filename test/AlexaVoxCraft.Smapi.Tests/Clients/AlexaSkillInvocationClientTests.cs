using System.Net;
using AlexaVoxCraft.Http.TestKit.Extensions;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;

namespace AlexaVoxCraft.Smapi.Tests.Clients;

public sealed class AlexaSkillInvocationClientTests
{
    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_RequestIsValid_ReturnsResponse(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/{stage}/invocations";
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel,
            req => req.RequestUri?.PathAndQuery == expectedUri);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_UsesPostMethod(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel,
            req => req.Method == HttpMethod.Post);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_WhenNotFound_ReturnsNull(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest)
    {
        handler.ReturnsResponse(HttpStatusCode.NotFound);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_WithDefaultRegion_UsesDefaultEndpointRegion(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, InvocationRegion.Default, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory]
    [InlineSkillInvocationClientAutoData(InvocationRegion.NA)]
    [InlineSkillInvocationClientAutoData(InvocationRegion.EU)]
    [InlineSkillInvocationClientAutoData(InvocationRegion.FE)]
    public async Task InvokeAsync_WithSpecificRegion_CompletesSuccessfully(
        InvocationRegion region,
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, region, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(responseModel);
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_WithDevelopmentStage_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/development/invocations";
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel,
            req => req.RequestUri?.PathAndQuery == expectedUri);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, "development", skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_WithLiveStage_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        var expectedUri = $"/v2/skills/{skillId}/stages/live/invocations";
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel,
            req => req.RequestUri?.PathAndQuery == expectedUri);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, "live", skillRequest, ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, SkillInvocationClientAutoData]
    public async Task InvokeAsync_ResponseContainsStatus_ReturnsCorrectStatus(
        [Frozen] HttpMessageHandler handler,
        AlexaSkillInvocationClient client,
        string skillId,
        string stage,
        SkillRequest skillRequest,
        SkillInvocationResponse<SkillResponse> responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var result = await client.InvokeAsync<SkillRequest, SkillResponse>(skillId, stage, skillRequest, ct: TestContext.Current.CancellationToken);

        result!.Status.Should().Be(responseModel.Status);
    }
}
