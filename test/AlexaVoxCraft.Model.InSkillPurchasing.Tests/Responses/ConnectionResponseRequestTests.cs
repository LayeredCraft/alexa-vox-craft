using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Responses;

public class ConnectionResponseRequestTests : TestBase<ConnectionResponseRequestTests>
{
    [Fact]
    public async Task ConnectionResponseRequest_Deserializes()
    {
        var json = Fx("Responses/ConnectionResponseRequest.json");
        var request = JsonSerializer.Deserialize<ConnectionResponseRequest<ConnectionResponsePayload>>(json, ClientOptions);

        request.Should().NotBeNull();
        request.Payload.Should().NotBeNull();

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task ConnectionResponseRequest_BuyDeclined_Deserializes()
    {
        var json = Fx("Components/ConnectionResponsePayload_Declined.json");
        var request = JsonSerializer.Deserialize<ConnectionResponseRequest<ConnectionResponsePayload>>(json, ClientOptions);

        request.Should().NotBeNull();
        request!.Name.Should().Be("Buy");
        request.Payload.PurchaseResult.Should().Be("DECLINED");

        await TestHelper.VerifyRequestObject(request);
    }
}