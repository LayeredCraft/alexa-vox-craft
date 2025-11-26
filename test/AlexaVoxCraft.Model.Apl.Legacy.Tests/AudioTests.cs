using AlexaVoxCraft.Model.Apl.Audio;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class AudioTests
{
    private readonly ITestOutputHelper _output;

    public AudioTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void APLADocument()
    {
        Utility.AssertSerialization<APLADocument>("APLADocument.json", _output);
    }

    [Fact]
    public void APLARenderDocument()
    {
        Utility.AssertSerialization<RenderDocumentDirective>("APLARenderDocument.json", _output);
    }

    [Fact]
    public void Audio()
    {
        Utility.AssertSerialization<Audio.Audio>("Audio_AudioFilters.json", _output);
    }

    [Fact]
    public void Mixer()
    {
        Utility.AssertSerialization<Mixer>("Audio_Mixer.json");
    }

    [Fact]
    public void Selector()
    {
        Utility.AssertSerialization<Selector>("Audio_Selector.json");
    }

    [Fact]
    public void Sequencer()
    {
        Utility.AssertSerialization<Sequencer>("Audio_Sequencer.json");
    }

    [Fact]
    public void Silence()
    {
        Utility.AssertSerialization<Silence>("Audio_Silence.json");
    }

    [Fact]
    public void Speech()
    {
        Utility.AssertSerialization<Speech>("Audio_Speech.json");
    }
}