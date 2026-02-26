using System.Net;
using AlexaVoxCraft.Http.TestKit.Extensions;
using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.Attributes;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Clients;

public sealed class InSkillPurchasingClientTests
{
    private const string Base = "/v1/users/~current/skills/~current";

    [Theory, IspClientAutoData]
    public async Task GetProductsAsync_RequestIsValid_ReturnsProducts(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        ProductResponse response)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, response);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, IspClientAutoData]
    public async Task GetProductsAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        ProductResponse response)
    {
        var expectedPath = $"{Base}/inSkillProducts";
        handler.ReturnsResponse(HttpStatusCode.OK, response,
            req => req.RequestUri?.AbsolutePath == expectedPath);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetProductsAsync_WhenNotFound_ReturnsNull(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut)
    {
        handler.ReturnsResponse(HttpStatusCode.NotFound);

        var result = await sut.GetProductsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetProductAsync_RequestIsValid_ReturnsProduct(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        string productId,
        Product response)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, response);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, IspClientAutoData]
    public async Task GetProductAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        string productId,
        Product response)
    {
        var expectedPath = $"{Base}/inSkillProducts/{productId}";
        handler.ReturnsResponse(HttpStatusCode.OK, response,
            req => req.RequestUri?.AbsolutePath == expectedPath);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetProductAsync_WhenNotFound_ReturnsNull(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        string productId)
    {
        handler.ReturnsResponse(HttpStatusCode.NotFound);

        var result = await sut.GetProductAsync(productId, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetPurchasingEnabledAsync_RequestIsValid_ReturnsPurchasingEnabled(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        PurchasingEnabled response)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, response);

        var result = await sut.GetPurchasingEnabledAsync(TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, IspClientAutoData]
    public async Task GetPurchasingEnabledAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        PurchasingEnabled response)
    {
        var expectedPath = $"{Base}/settings/voicePurchasing.enabled";
        handler.ReturnsResponse(HttpStatusCode.OK, response,
            req => req.RequestUri?.AbsolutePath == expectedPath);

        var result = await sut.GetPurchasingEnabledAsync(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetTransactionsAsync_RequestIsValid_ReturnsTransactions(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        TransactionResponse response)
    {
        handler.ReturnsResponse(HttpStatusCode.OK, response);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(response);
    }

    [Theory, IspClientAutoData]
    public async Task GetTransactionsAsync_WithValidUri_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut,
        TransactionResponse response)
    {
        var expectedPath = $"{Base}/inSkillProductsTransactions";
        handler.ReturnsResponse(HttpStatusCode.OK, response,
            req => req.RequestUri?.AbsolutePath == expectedPath);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Theory, IspClientAutoData]
    public async Task GetTransactionsAsync_WhenNotFound_ReturnsNull(
        [Frozen] HttpMessageHandler handler,
        InSkillPurchasingClient sut)
    {
        handler.ReturnsResponse(HttpStatusCode.NotFound);

        var result = await sut.GetTransactionsAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }
}