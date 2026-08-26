using System.Net;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Models.InteractionModel;
using AlexaVoxCraft.Smapi.Tests.TestKit;
using Compono;
using Compono.Http;
using Compono.XunitV3;

namespace AlexaVoxCraft.Smapi.Tests.Clients;

// ADR-0002 Amendment 3/ADR-0052 (Part B): AlexaInteractionModelClient composes directly - its
// constructor's HttpClient parameter has 3 accessible constructors, which SmapiHttpTestProfile's
// own builder.For<HttpClient>().UseConstructor<HttpMessageHandler, bool>() resolves at compile
// time (CMP0001 no longer fires for HttpClient anywhere this profile applies). The profile's
// existing Register<HttpClient> still supplies the actual runtime value (a registration outranks
// a generated plan) - already built via HttpTestHarness.CreateClient(...), so the shared handler
// identity, BaseAddress, and disposeHandler:false semantics this class used to hand-construct via
// its own CreateClient(handler) helper are unchanged, just no longer duplicated per test.
public sealed class AlexaInteractionModelClientTests
{
    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAsync_RequestIsValid_ReturnsModel(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(responseModel);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition responseModel)
    {
        var expectedUri = $"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}";
        var registration = handler.OnGet(expectedUri).RespondJson(responseModel);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAsync_WhenNotFound_ReturnsNull(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale)
    {
        handler.OnGet(Match.Any<string>()).Respond(HttpStatusCode.NotFound);

        var model = await client.GetAsync(skillId, stage, locale, TestContext.Current.CancellationToken);

        model.Should().BeNull();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAsync_WithDevelopmentStage_ReturnsModel(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(responseModel);

        var model = await client.GetAsync(skillId, "development", locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAsync_WithLiveStage_ReturnsModel(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition responseModel)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(responseModel);

        var model = await client.GetAsync(skillId, "live", locale, TestContext.Current.CancellationToken);

        model.Should().BeEquivalentTo(responseModel);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task UpdateAsync_WithValidModel_CompletesSuccessfully(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        var registration = handler.OnPut(Match.Any<string>()).Respond(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task UpdateAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        var expectedUri = $"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}";
        var registration = handler.OnPut(expectedUri).Respond(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task UpdateAsync_WithDevelopmentStage_CompletesSuccessfully(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition model)
    {
        var registration = handler.OnPut(Match.Any<string>()).Respond(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, "development", locale, model, TestContext.Current.CancellationToken);

        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task UpdateAsync_WithLiveStage_CompletesSuccessfully(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string locale,
        InteractionModelDefinition model)
    {
        var registration = handler.OnPut(Match.Any<string>()).Respond(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, "live", locale, model, TestContext.Current.CancellationToken);

        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task UpdateAsync_WithComplexModel_SerializesCorrectly(
        [Shared] HttpTestHarness handler,
        AlexaInteractionModelClient client,
        string skillId,
        string stage,
        string locale,
        InteractionModelDefinition model)
    {
        var registration = handler.OnPut(Match.Any<string>()).Respond(HttpStatusCode.NoContent);

        await client.UpdateAsync(skillId, stage, locale, model, TestContext.Current.CancellationToken);

        registration.Verify().Once();
    }
}
