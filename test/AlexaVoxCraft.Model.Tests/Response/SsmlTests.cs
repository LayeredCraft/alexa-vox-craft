using System.Xml.Linq;
using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Coverage for the SSML (Speech Synthesis Markup Language) builder classes. <see cref="ISsml.ToXml"/>
/// is a one-directional string builder with no deserialize counterpart, so the request/response
/// direction rule doesn't apply here.
/// </summary>
public sealed class SsmlTests
{
    [Fact]
    public void Speech_WithNoText_ThrowsOnToXml()
    {
        var speech = new Speech();

        var act = () => speech.ToXml();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Speech_GeneratesSpeakAndElements()
    {
        var speech = new Speech();
        speech.Elements.Add(new PlainText("hello"));

        speech.ToXml().Should().Be("<speak>hello</speak>");
    }

    [Fact]
    public void PlainText_GeneratesText()
    {
        CompareXml("Hello World", new PlainText("Hello World"));
    }

    [Fact]
    public void Sentence_WithText_GeneratesS()
    {
        CompareXml("<s>Hello World</s>", new Sentence("Hello World"));
    }

    [Fact]
    public void Paragraph_GeneratesP()
    {
        var paragraph = new Paragraph();
        paragraph.Elements.Add(new PlainText("Hello World"));

        CompareXml("<p>Hello World</p>", paragraph);
    }

    [Fact]
    public void Break_GeneratesBreak()
    {
        CompareXml("<break />", new Break());
    }

    [Fact]
    public void Break_GeneratesTimeAttribute()
    {
        CompareXml("""<break time="3s" />""", new Break { Time = "3s" });
    }

    [Fact]
    public void Break_GeneratesStrength()
    {
        CompareXml("""<break strength="x-weak" />""", new Break { Strength = BreakStrength.ExtraWeak });
    }

    [Fact]
    public void SayAs_GeneratesSayAs()
    {
        CompareXml("""<say-as interpret-as="spell-out">Hello World</say-as>""", new SayAs("Hello World", InterpretAs.SpellOut));
    }

    [Fact]
    public void SayAs_GeneratesFormat()
    {
        CompareXml(
            """<say-as interpret-as="spell-out" format="ymd">Hello World</say-as>""",
            new SayAs("Hello World", InterpretAs.SpellOut) { Format = "ymd" });
    }

    [Fact]
    public void Word_GeneratesW()
    {
        CompareXml("""<w role="amazon:VB">world</w>""", new Word("world", WordRole.Verb));
    }

    [Fact]
    public void Sub_GeneratesSub()
    {
        CompareXml("""<sub alias="magnesium">Mg</sub>""", new Sub("Mg", "magnesium"));
    }

    [Fact]
    public void Prosody_GeneratesProsody()
    {
        var prosody = new Prosody
        {
            Rate = ProsodyRate.Percent(150),
            Pitch = ProsodyPitch.ExtraLow,
            Volume = ProsodyVolume.Decibel(-5)
        };
        prosody.Elements.Add(new PlainText("Hello World"));

        CompareXml("""<prosody rate="150%" pitch="x-low" volume="-5dB">Hello World</prosody>""", prosody);
    }

    [Fact]
    public void Emphasis_GeneratesEmphasis()
    {
        var emphasis = new Emphasis("Hello World") { Level = EmphasisLevel.Strong };

        CompareXml("""<emphasis level="strong">Hello World</emphasis>""", emphasis);
    }

    [Fact]
    public void Phoneme_GeneratesPhoneme()
    {
        CompareXml(
            """<phoneme alphabet="ipa" ph="pɪˈkɑːn">pecan</phoneme>""",
            new Phoneme("pecan", PhonemeAlphabet.International, "pɪˈkɑːn"));
    }

    [Fact]
    public void Audio_GeneratesAudio()
    {
        var audio = new Audio("http://example.com/example.mp3");
        audio.Elements.Add(new PlainText("Hello World"));

        CompareXml("""<audio src="http://example.com/example.mp3">Hello World</audio>""", audio);
    }

    [Fact]
    public void AmazonEffect_GeneratesAmazonEffect()
    {
        var speech = new Speech();
        speech.Elements.Add(new AmazonEffect("Hello World"));

        speech.ToXml().Should().Be("""<speak><amazon:effect name="whispered">Hello World</amazon:effect></speak>""");
    }

    [Fact]
    public void TerseAndVerboseConstruction_ProduceIdenticalXml()
    {
        const string speech1 = "Welcome to";
        const string speech2 = "the most awesome game ever";
        const string speech3 = "what do you want to do?";

        var verbose = new Speech
        {
            Elements = new List<ISsml>
            {
                new Paragraph
                {
                    Elements = new List<IParagraphSsml>
                    {
                        new PlainText(speech1),
                        new Prosody { Rate = ProsodyRate.Fast, Elements = new List<ISsml> { new Sentence(speech2) } },
                        new Sentence(speech3)
                    }
                }
            }
        };

        var terse = new Speech(
            new Paragraph(
                new PlainText(speech1),
                new Prosody(new Sentence(speech2)) { Rate = ProsodyRate.Fast },
                new Sentence(speech3)));

        terse.ToXml().Should().Be(verbose.ToXml());
    }

    [Fact]
    public void VoiceAndLang_GenerateCorrectly()
    {
        var speech = new Voice("Celine", new Lang("fr-FR", new PlainText("Je ne parle pas francais")));

        CompareXml("""<voice name="Celine"><lang xml:lang="fr-FR">Je ne parle pas francais</lang></voice>""", speech);
    }

    [Fact]
    public void AlexaName_GeneratesAlexaName()
    {
        var speech = new Speech();
        speech.Elements.Add(new AlexaName("amzn1.ask.person.ABCDEF"));

        speech.ToXml().Should().Be("""<speak><alexa:name type="first" personId="amzn1.ask.person.ABCDEF" /></speak>""");
    }

    [Fact]
    public void AmazonDomain_GeneratesDomain()
    {
        var domain = new AmazonDomain(DomainName.News);
        domain.Elements.Add(new PlainText("A miniature manuscript"));
        var speech = new Speech();
        speech.Elements.Add(domain);

        speech.ToXml().Should().Be("""<speak><amazon:domain name="news">A miniature manuscript</amazon:domain></speak>""");
    }

    [Fact]
    public void AmazonEmotion_GeneratesEmotion()
    {
        var emotion = new AmazonEmotion(EmotionName.Excited, EmotionIntensity.Medium);
        emotion.Elements.Add(new PlainText("Christina wins this round!"));
        var speech = new Speech();
        speech.Elements.Add(emotion);

        speech.ToXml().Should().Be("""<speak><amazon:emotion name="excited" intensity="medium">Christina wins this round!</amazon:emotion></speak>""");
    }

    private static void CompareXml(string expected, ISsml ssml)
    {
        ssml.ToXml().ToString(SaveOptions.DisableFormatting).Should().Be(expected);
    }
}
