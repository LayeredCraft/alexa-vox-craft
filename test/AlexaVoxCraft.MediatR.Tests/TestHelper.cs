using System.Text.Json;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Tests;

/// <summary>
/// PLAN-0051 (Compono ecosystem migration, TestKit slice 1): explicit replacements for the old
/// AlexaVoxCraft.TestKit SkillRequestSpecimenBuilder/OptionsSpecimenBuilder/
/// SkillServiceConfigurationSpecimenBuilder's parameter-name-keyed scenario dispatch (removed -
/// classified "Acceptable Compono-native alternative" per ADR-0029, not reproduced in Compono).
/// A test that needs a specific scenario now builds it explicitly here rather than relying on a
/// composed parameter's own name.
/// </summary>
public static class TestHelper
{
    public static SkillRequest ForRequest(Request request) => new()
    {
        Context = new Context
        {
            System = new AlexaSystem
            {
                Application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" },
            },
        },
        Session = new Session { Attributes = new Dictionary<string, JsonElement>() },
        Request = request,
    };

    public static IntentRequest IntentRequest(string intentName) => new()
    {
        Type = "IntentRequest",
        RequestId = Guid.NewGuid().ToString(),
        Timestamp = DateTime.UtcNow,
        Locale = "en-US",
        Intent = new Intent { Name = intentName, ConfirmationStatus = "NONE" },
    };

    public static SessionEndedRequest SessionEndedRequest() => new()
    {
        Type = "SessionEndedRequest",
        RequestId = Guid.NewGuid().ToString(),
        Timestamp = DateTime.UtcNow,
        Locale = "en-US",
        Reason = Reason.UserInitiated,
    };

    public static AudioPlayerRequest AudioPlayerRequest() => new()
    {
        Type = "AudioPlayer.PlaybackStopped",
        RequestId = Guid.NewGuid().ToString(),
        Timestamp = DateTime.UtcNow,
        Locale = "en-US",
    };

    public static DisplayElementSelectedRequest DisplayRequest() => new()
    {
        Type = "Display.ElementSelected",
        RequestId = Guid.NewGuid().ToString(),
        Timestamp = DateTime.UtcNow,
        Locale = "en-US",
    };

    public static IOptions<SkillServiceConfiguration> SkillOptions(
        string? skillId = "amzn1.ask.skill.default-test-id",
        string? defaultVoiceName = null) =>
        Options.Create(new SkillServiceConfiguration
        {
            SkillId = skillId,
            CustomUserAgent = "TestAgent/1.0",
            DefaultVoiceName = defaultVoiceName,
        });

    public static IOptions<SkillServiceConfiguration> SkillOptionsWithNullValue() =>
        Options.Create<SkillServiceConfiguration>(null!);
}
