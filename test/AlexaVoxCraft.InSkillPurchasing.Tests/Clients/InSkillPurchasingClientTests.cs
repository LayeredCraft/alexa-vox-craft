using System.Net;
using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.InSkillPurchasing.Tests.TestKit;
using Compono;
using Compono.Http;
using Compono.XunitV3;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Clients;

// Mirrors AlexaVoxCraft.Smapi.Tests.Clients.AlexaInteractionModelClientTests: the shared profile
// composes InSkillPurchasingClient with an HttpClient created from the same shared handler the test
// configures and verifies.
public sealed class InSkillPurchasingClientTests
{
    private const string Base = "/v1/users/~current/skills/~current";

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductsAsync_RequestIsValid_ReturnsProducts(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        ProductResponse response)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(response);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductsAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        ProductResponse response)
    {
        var expectedPath = $"{Base}/inSkillProducts";
        var registration = handler.OnGet(expectedPath).RespondJson(response);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductsAsync_WhenNotFound_ReturnsNull(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut)
    {
        handler.OnGet(Match.Any<string>()).Respond(HttpStatusCode.NotFound);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductAsync_RequestIsValid_ReturnsProduct(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        string productId,
        Product response)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(response);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        string productId,
        Product response)
    {
        var expectedPath = $"{Base}/inSkillProducts/{productId}";
        var registration = handler.OnGet(expectedPath).RespondJson(response);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetProductAsync_WhenNotFound_ReturnsNull(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        string productId)
    {
        handler.OnGet(Match.Any<string>()).Respond(HttpStatusCode.NotFound);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetPurchasingEnabledAsync_RequestIsValid_ReturnsPurchasingEnabled(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        PurchasingEnabled response)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(response);

        var result = await sut.GetPurchasingEnabledAsync(TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetPurchasingEnabledAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        PurchasingEnabled response)
    {
        var expectedPath = $"{Base}/settings/voicePurchasing.enabled";
        var registration = handler.OnGet(expectedPath).RespondJson(response);

        var result = await sut.GetPurchasingEnabledAsync(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetTransactionsAsync_RequestIsValid_ReturnsTransactions(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        TransactionResponse response)
    {
        handler.OnGet(Match.Any<string>()).RespondJson(response);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetTransactionsAsync_WithValidUri_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut,
        TransactionResponse response)
    {
        var expectedPath = $"{Base}/inSkillProductsTransactions";
        var registration = handler.OnGet(expectedPath).RespondJson(response);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        registration.Verify().Once();
    }

    [Theory, Compose<IspHttpTestProfile>]
    public async Task GetTransactionsAsync_WhenNotFound_ReturnsNull(
        [Shared] HttpTestHarness handler,
        InSkillPurchasingClient sut)
    {
        handler.OnGet(Match.Any<string>()).Respond(HttpStatusCode.NotFound);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }
}
