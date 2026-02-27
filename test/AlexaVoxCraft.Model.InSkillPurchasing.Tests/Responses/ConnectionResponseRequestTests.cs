using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Responses;

public class ConnectionResponseRequestTests : TestBase<ConnectionResponseRequestTests>
{
    [Fact]
    public async Task ConnectionResponseRequest_Deserializes()
    {
        var json = Fx("Response/ConnectionResponseRequest.json");
        var request = JsonSerializer.Deserialize<ConnectionResponseRequest<ConnectionResponsePayload>>(json, ClientOptions);
        
        request.Should().NotBeNull();
        request.Payload.Should().NotBeNull();
        
        await TestHelper.VerifyRequestObject(request);
    }
}