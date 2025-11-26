using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

public sealed class SkillRequestTests() : TestBase
{
    [Fact]
    public async Task IntentRequest_Deserializes()
    {
        var json = Fx("Requests/IntentRequest.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope.Request.Should().BeOfType<IntentRequest>().Subject.Intent.Name.Should().Be("AnswerIntent");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task LaunchRequest_Deserializes()
    {
        var json = Fx("Requests/LaunchRequest.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope.Request.Should().BeOfType<LaunchRequest>();

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task SessionEndedRequest_Deserializes()
    {
        var json = Fx("Requests/SessionEndedRequest.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope.Request.Should().BeOfType<SessionEndedRequest>();

        await TestHelper.VerifyRequestObject(envelope);
    }
}