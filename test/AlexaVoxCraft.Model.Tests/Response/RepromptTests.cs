using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for <see cref="Reprompt"/> construction. Reprompt is a response object
/// the skill only ever builds and sends, so it is tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class RepromptTests
{
    [Fact]
    public void StringConstructor_GeneratesPlainTextOutputSpeech()
    {
        var result = new Reprompt("text");

        var plainText = result.OutputSpeech.Should().BeOfType<PlainTextOutputSpeech>().Subject;
        plainText.Text.Should().Be("text");
    }

    [Fact]
    public void SpeechConstructor_GeneratesSsmlOutputSpeech()
    {
        var speech = new Speech(new PlainText("text"));

        var result = new Reprompt(speech);

        var ssmlText = result.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlText.Ssml.Should().Be(speech.ToXml());
    }

    [Fact]
    public void SsmlOutputSpeech_StringConstructor_SetsSsmlFromSpeechXml()
    {
        var xml = new Speech(new PlainText("testing output")).ToXml();

        var ssmlText = new SsmlOutputSpeech(xml);

        ssmlText.Ssml.Should().Be(xml);
    }
}
