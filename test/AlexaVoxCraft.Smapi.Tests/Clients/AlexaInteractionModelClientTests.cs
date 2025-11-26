using System.Net;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.InteractionModel;
using AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;
using AlexaVoxCraft.Smapi.Tests.TestKit.Extensions;

namespace AlexaVoxCraft.Smapi.Tests.Clients;

public sealed class AlexaInteractionModelClientTests
{
    [Theory, ClientAutoData]
    public async Task GetAsync_RequestIsValid_ReturnsModel(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler
            .ReturnsResponse(HttpStatusCode.OK, responseModel);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, ClientAutoData]
    public async Task GetAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition responseModel)
    {
        var expectedUri = $"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}";
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel,
            req => req.RequestUri?.PathAndQuery == expectedUri);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().NotBeNull();
    }

    [Theory, ClientAutoData]
    public async Task GetAsync_WhenNotFound_ReturnsNull(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale)
    {
        handler.ReturnsResponse(HttpStatusCode.NotFound);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().BeNull();
    }

    [Theory, ClientAutoData]
    public async Task GetAsync_WithDevelopmentStage_ReturnsModel(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var model = await client.GetAsync(skillId, "development", locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, ClientAutoData]
    public async Task GetAsync_WithLiveStage_ReturnsModel(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, responseModel);

        var model = await client.GetAsync(skillId, "live", locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, ClientAutoData]
    public async Task UpdateAsync_WithValidModel_CompletesSuccessfully(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        handler.ReturnsResponse(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        handler.Received();
    }

    [Theory, ClientAutoData]
    public async Task UpdateAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        var expectedUri = $"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}";
        handler.ReturnsResponse(HttpStatusCode.NoContent,
            predicate: req => req.RequestUri?.PathAndQuery == expectedUri && req.Method == HttpMethod.Put);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        handler.Received();
    }

    [Theory, ClientAutoData]
    public async Task UpdateAsync_WithDevelopmentStage_CompletesSuccessfully(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition model)
    {
        handler.ReturnsResponse(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, "development", locale, model, TestContext.Current.CancellationToken);

        handler.Received();
    }

    [Theory, ClientAutoData]
    public async Task UpdateAsync_WithLiveStage_CompletesSuccessfully(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition model)
    {
        handler.ReturnsResponse(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, "live", locale, model, TestContext.Current.CancellationToken);

        handler.Received();
    }

    [Theory, ClientAutoData]
    public async Task UpdateAsync_WithComplexModel_SerializesCorrectly(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        handler.ReturnsResponse(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        handler.Received();
    }
}