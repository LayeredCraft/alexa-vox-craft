using System.Text.Json;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Envelope-level coverage for <see cref="SkillResponse"/>. Responses are objects the skill only
/// ever builds and sends, so they are tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class SkillResponseTests() : TestBase<SkillResponseTests>
{
    [Fact]
    public async Task SkillResponse_Serializes_WithSimpleCard()
    {
        var response = new SkillResponse
        {
            Version = "1.0",
            Response = new ResponseBody
            {
                Card = new SimpleCard
                {
                    Title = "Disney Trivia",
                    Content = "Question 1. When Disney World opened, who gave the opening speech? 1.  Roy Disney. 2.  Lillian Disney. 3.  The president. 4.  Walt Disney. "
                },
                ShouldEndSession = false
            }
        };

        await TestHelper.VerifySerializedObject(response, AlexaJson, "SkillResponse_WithSimpleCard");
    }

    [Fact]
    public async Task SkillResponse_Serializes_WithShouldEndSessionTrue()
    {
        var response = new SkillResponse
        {
            Version = "1.0",
            Response = new ResponseBody
            {
                ShouldEndSession = true
            }
        };

        await TestHelper.VerifySerializedObject(response, AlexaJson, "SkillResponse_ShouldEndSessionTrue");
    }

    [Fact]
    public async Task SkillResponse_Serializes_WithSpeechCardAndSessionAttributes()
    {
        var response = new SkillResponse
        {
            Version = "1.0",
            SessionAttributes = new Dictionary<string, JsonElement>
            {
                ["supportedHoriscopePeriods"] = JsonSerializer.SerializeToElement(new { daily = true, weekly = false, monthly = false })
            },
            Response = new ResponseBody
            {
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = "Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless. Can I help you with anything else?"
                },
                Card = new SimpleCard
                {
                    Title = "Horoscope",
                    Content = "Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless."
                },
                ShouldEndSession = false
            }
        };

        await TestHelper.VerifySerializedObject(response, AlexaJson, "SkillResponse_WithSpeechCardAndSessionAttributes");
    }
}
