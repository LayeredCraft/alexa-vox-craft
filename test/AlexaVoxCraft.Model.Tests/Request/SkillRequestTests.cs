using System.Text.Json;
using AlexaVoxCraft.Model;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

public sealed class SkillRequestTests() : TestBase<SkillRequestTests>
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

    [Fact]
    public async Task LaunchRequest_FreshSession_Deserializes()
    {
        var envelope = Deserialize("Requests/LaunchRequest_FreshSession.json");

        envelope.Request.Should().BeOfType<LaunchRequest>();
        envelope.Session.New.Should().BeTrue();

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task IntentRequest_WithDialogState_Deserializes()
    {
        var envelope = Deserialize("Requests/IntentRequest_WithDialogState.json");

        var request = envelope.Request.Should().BeOfType<IntentRequest>().Subject;
        request.DialogState.Should().Be(DialogState.InProgress);
        request.Intent.ConfirmationStatus.Should().Be(ConfirmationStatus.Denied);
        request.Intent.Slots["ZodiacSign"].ConfirmationStatus.Should().Be(ConfirmationStatus.Confirmed);

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task IntentRequest_AnswerIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AnswerIntent.json", "AnswerIntent");

    [Fact]
    public async Task IntentRequest_CategorySelectionIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_CategorySelectionIntent.json", "CategorySelectionIntent");

    [Fact]
    public async Task IntentRequest_LeaderboardIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_LeaderboardIntent.json", "LeaderboardIntent");

    [Fact]
    public async Task IntentRequest_PlayerStatsIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_PlayerStatsIntent.json", "PlayerStatsIntent");

    [Fact]
    public async Task IntentRequest_DontKnowIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_DontKnowIntent.json", "DontKnowIntent");

    [Fact]
    public async Task IntentRequest_AmazonHelpIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonHelpIntent.json", "AMAZON.HelpIntent");

    [Fact]
    public async Task IntentRequest_AmazonNoIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonNoIntent.json", "AMAZON.NoIntent");

    [Fact]
    public async Task IntentRequest_AmazonRepeatIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonRepeatIntent.json", "AMAZON.RepeatIntent");

    [Fact]
    public async Task IntentRequest_AmazonStopIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonStopIntent.json", "AMAZON.StopIntent");

    [Fact]
    public async Task IntentRequest_AmazonUnknownIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonUnknownIntent.json", "AMAZON.UnknownIntent");

    [Fact]
    public async Task IntentRequest_AmazonYesIntent_Deserializes() => await VerifyIntent("Requests/IntentRequest_AmazonYesIntent.json", "AMAZON.YesIntent");

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
