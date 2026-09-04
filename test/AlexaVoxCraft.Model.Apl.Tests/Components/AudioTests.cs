using AlexaVoxCraft.Model.Apl.Audio;
using AlexaVoxCraft.Model.Apl.Audio.Filters;
using AudioComponent = AlexaVoxCraft.Model.Apl.Audio.Audio;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Component-level coverage for the APLA (APL Audio) document tree: <see cref="APLADocument"/> and
/// its component types. Response-side, serialize-only, consistent with every other APL type in this
/// project.
/// </summary>
public sealed class AudioTests() : TestBase<AudioTests>
{
    [Fact]
    public async Task APLADocument_Serializes()
    {
        var document = new APLADocument
        {
            MainTemplate = new AudioLayout(new Speech { Content = "Welcome to the skill.", ContentType = SpeechContentType.PlainText })
        };

        await TestHelper.VerifySerializedObject(document, AlexaJson, "APLADocument");
    }

    [Fact]
    public async Task Audio_Serializes_WithFilters()
    {
        var audio = new AudioComponent
        {
            Source = "https://example.com/audio/clip.mp3",
            Filters = [new FadeIn { Duration = 1000 }, new Trim { Start = 0, End = 5000 }]
        };

        await TestHelper.VerifySerializedObject(audio, AlexaJson, "Audio_WithFilters");
    }

    [Fact]
    public async Task Mixer_Serializes()
    {
        var mixer = new Mixer { Items = [new Speech { Content = "hello" }, new Silence { Duration = 500 }] };

        await TestHelper.VerifySerializedObject(mixer, AlexaJson, "Mixer");
    }

    [Fact]
    public async Task Selector_Serializes()
    {
        var selector = new Selector
        {
            Strategy = SelectorStrategy.RandomItem,
            Items = [new Speech { Content = "option one" }, new Speech { Content = "option two" }]
        };

        await TestHelper.VerifySerializedObject(selector, AlexaJson, "Selector");
    }

    [Fact]
    public async Task Sequencer_Serializes()
    {
        var sequencer = new Sequencer { Items = [new Speech { Content = "first" }, new Speech { Content = "second" }] };

        await TestHelper.VerifySerializedObject(sequencer, AlexaJson, "Sequencer");
    }

    [Fact]
    public async Task Silence_Serializes()
    {
        var silence = new Silence { Duration = 500 };

        await TestHelper.VerifySerializedObject(silence, AlexaJson, "Silence");
    }

    [Fact]
    public async Task Speech_Serializes()
    {
        var speech = new Speech { Content = "Hello world", ContentType = SpeechContentType.Ssml };

        await TestHelper.VerifySerializedObject(speech, AlexaJson, "Speech");
    }
}
