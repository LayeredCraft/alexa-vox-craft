using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for <see cref="IOutputSpeech"/> types. Output speech is a response
/// object the skill only ever builds and sends, so it is tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class OutputSpeechTests() : TestBase<OutputSpeechTests>
{
    [Fact]
    public void PlainTextConstructor_SetsText()
    {
        var speech = new PlainTextOutputSpeech("testing output");

        speech.Text.Should().Be("testing output");
    }

    [Fact]
    public async Task PlainTextOutputSpeech_Serializes()
    {
        var speech = new PlainTextOutputSpeech { Text = "text content" };

        await TestHelper.VerifySerializedObject(speech, AlexaJson, "PlainTextOutputSpeech");
    }

    [Fact]
    public async Task PlainTextOutputSpeech_Serializes_WithPlayBehavior()
    {
        var speech = new PlainTextOutputSpeech { Text = "text content", PlayBehavior = PlayBehavior.ReplaceAll };

        await TestHelper.VerifySerializedObject(speech, AlexaJson, "PlainTextOutputSpeech_WithPlayBehavior");
    }

    [Fact]
    public async Task SsmlOutputSpeech_Serializes()
    {
        var speech = new SsmlOutputSpeech { Ssml = "ssml content" };

        await TestHelper.VerifySerializedObject(speech, AlexaJson, "SsmlOutputSpeech");
    }

    [Fact]
    public async Task SsmlOutputSpeech_Serializes_WithPlayBehavior()
    {
        var speech = new SsmlOutputSpeech { Ssml = "ssml content", PlayBehavior = PlayBehavior.ReplaceEnqueued };

        await TestHelper.VerifySerializedObject(speech, AlexaJson, "SsmlOutputSpeech_WithPlayBehavior");
    }
}
