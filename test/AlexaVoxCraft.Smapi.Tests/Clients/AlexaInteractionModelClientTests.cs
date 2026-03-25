using System.Net;
using AlexaVoxCraft.Http.TestKit.Extensions;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.InteractionModel;
using AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;

namespace AlexaVoxCraft.Smapi.Tests.Clients;

public sealed class AlexaInteractionModelClientTests
{
    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
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

    [Theory, SmapiClientAutoData]
    public async Task UpdateAsync_WithLocalizedModel_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition definition)
    {
        var model = new LocalizedInteractionModel(locale, definition);
        var expectedUri = $"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}";
        handler.ReturnsResponse(HttpStatusCode.NoContent,
            predicate: req => req.RequestUri?.PathAndQuery == expectedUri && req.Method == HttpMethod.Put);

        await client.UpdateAsync(skillId, stage, model, TestContext.Current.CancellationToken);

        handler.Received();
    }

    [Theory, SmapiClientAutoData]
    public async Task UpdateAsync_WithNullLocalizedModel_ThrowsArgumentNullException(
        AlexaInteractionModelClient client,
        string skillId,
        string stage)
    {
        var act = () => client.UpdateAsync(skillId, stage, (LocalizedInteractionModel)null!, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("model");
    }

    [Theory, SmapiClientAutoData]
    public async Task UpdateAllAsync_WithMultipleModels_CallsEndpointForEach(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        InteractionModelDefinition definition1,
        InteractionModelDefinition definition2)
    {
        var models = new[]
        {
            new LocalizedInteractionModel("en-US", definition1),
            new LocalizedInteractionModel("en-GB", definition2)
        };
        handler.ReturnsResponse(HttpStatusCode.NoContent);

        await client.UpdateAllAsync(skillId, stage, models, TestContext.Current.CancellationToken);

        handler.Received(2);
    }

    [Theory, SmapiClientAutoData]
    public async Task UpdateAllAsync_WithEmptyModels_CompletesWithoutCalling(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage)
    {
        await client.UpdateAllAsync(skillId, stage, [], TestContext.Current.CancellationToken);

        handler.DidNotReceive();
    }

    [Theory, SmapiClientAutoData]
    public async Task UpdateAllAsync_WhenSomeLocalesFail_ThrowsAggregateExceptionAfterAttemptingAll(
        [Frozen] HttpMessageHandler handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        InteractionModelDefinition definition1,
        InteractionModelDefinition definition2,
        InteractionModelDefinition definition3)
    {
        var models = new[]
        {
            new LocalizedInteractionModel("en-US", definition1),
            new LocalizedInteractionModel("en-CA", definition2),
            new LocalizedInteractionModel("en-GB", definition3)
        };

        handler.ReturnsResponse(HttpStatusCode.NoContent, predicate: req =>
            req.RequestUri!.PathAndQuery.EndsWith("/en-US"));

        handler.ReturnsResponse(HttpStatusCode.InternalServerError, predicate: req =>
            req.RequestUri!.PathAndQuery.EndsWith("/en-CA"));

        handler.ReturnsResponse(HttpStatusCode.InternalServerError, predicate: req =>
            req.RequestUri!.PathAndQuery.EndsWith("/en-GB"));

        var act = () => client.UpdateAllAsync(skillId, stage, models, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("Failed to update 2 locale(s).*");
    }
}