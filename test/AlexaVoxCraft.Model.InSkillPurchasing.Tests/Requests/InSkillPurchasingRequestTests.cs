using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Requests;

/// <summary>
/// Envelope-level coverage for the in-skill purchasing request flow: the custom intents that kick
/// off a purchase, and the Connections.Response the purchasing flow completes with.
/// </summary>
public sealed class InSkillPurchasingRequestTests() : TestBase<InSkillPurchasingRequestTests>
{
    [Fact]
    public async Task IntentRequest_BuyIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_BuyIntent.json", "BuyIntent");

    [Fact]
    public async Task IntentRequest_WhatCanIBuyIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_WhatCanIBuyIntent.json", "WhatCanIBuyIntent");

    [Fact]
    public async Task IntentRequest_ProductDetailIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_ProductDetailIntent.json", "ProductDetailIntent");

    [Fact]
    public async Task ConnectionsResponse_Buy_Deserializes()
    {
        var envelope = Deserialize("Requests/ConnectionsResponse_Buy.json");

        var response = envelope.Request.Should().BeOfType<ConnectionResponseRequest<ConnectionResponsePayload>>().Subject;
        response.Name.Should().Be("Buy");
        response.Payload.PurchaseResult.Should().Be("DECLINED");

        await TestHelper.VerifyRequestObject(envelope);
    }

    private async Task VerifyIntent(string fixturePath, string expectedIntentName)
    {
        var envelope = Deserialize(fixturePath);

        envelope.Request.Should().BeOfType<IntentRequest>().Subject.Intent.Name.Should().Be(expectedIntentName);

        await TestHelper.VerifyRequestObject(envelope);
    }

    private static SkillRequest Deserialize(string fixturePath)
    {
        var json = Fx(fixturePath);
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);
        envelope.Should().NotBeNull();
        return envelope!;
    }
}
