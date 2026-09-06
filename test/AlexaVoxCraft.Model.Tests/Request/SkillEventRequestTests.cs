using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

public sealed class SkillEventRequestTests() : TestBase<SkillEventRequestTests>
{
    [Fact]
    public async Task AccountLinkSkillEventRequest_Deserializes()
    {
        var envelope = Deserialize("Requests/SkillEventAccountLink.json");

        var request = envelope.Request.Should().BeOfType<AccountLinkSkillEventRequest>().Subject;
        request.Body.AccessToken.Should().Be("linkedAccessToken");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task PermissionSkillEventRequest_Deserializes()
    {
        var envelope = Deserialize("Requests/SkillEventPermissionChange.json");

        var request = envelope.Request.Should().BeOfType<PermissionSkillEventRequest>().Subject;
        request.Body.AcceptedPermissions.Should().ContainSingle()
            .Which.Scope.Should().Be("alexa::devices:all:geolocation:read");
        request.EventCreationTime.Should().NotBeNull();
        request.EventPublishingTime.Should().NotBeNull();

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task SkillEventRequest_Deserializes_AsNonSpecializedFallback()
    {
        var envelope = Deserialize("Requests/SkillEventNonSpecialized.json");

        envelope.Request.Should().BeOfType<SkillEventRequest>();

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
