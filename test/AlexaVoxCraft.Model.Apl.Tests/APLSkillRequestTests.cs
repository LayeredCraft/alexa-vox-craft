using System.Text.Json;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class APLSkillRequestTests : TestBase
{
    [Fact]
    public async Task UserTouchRequest_Deserializes()
    {
        var json = Fx("Requests/UserTouchRequest.json");
        var envelope = JsonSerializer.Deserialize<APLSkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope.Request.Should().BeOfType<UserEventRequest>();

        await TestHelper.VerifyRequestObject(envelope);
    }

}