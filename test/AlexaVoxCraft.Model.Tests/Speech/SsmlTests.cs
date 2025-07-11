﻿using System.Xml.Linq;
using AlexaVoxCraft.Model.Response.Ssml;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using SsmlSpeech = AlexaVoxCraft.Model.Response.Ssml.Speech;

namespace AlexaVoxCraft.Model.Tests.Speech;

public sealed class SsmlTests
{
    [Fact]
    public void Ssml_Error_With_No_Text()
    {
        var ssml = new SsmlSpeech();

        Assert.Throws<InvalidOperationException>(() => ssml.ToXml());
    }

    [Fact]
    public void Ssml_Generates_Speak_And_Elements()
    {
        const string expected = "<speak>hello</speak>";
        var ssml = new SsmlSpeech();

        ssml.Elements.Add(new PlainText("hello"));
        var actual = ssml.ToXml();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Ssml_PlainText_Generates_Text()
    {
        const string expected = "Hello World";

        var actual = new PlainText(expected);

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Sentence_With_Text_Generates_s()
    {
        const string expected = "<s>Hello World</s>";

        var actual = new Sentence("Hello World");

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Paragraph_Generates_p()
    {
        const string expected = "<p>Hello World</p>";

        var actual = new Paragraph();
        actual.Elements.Add(new PlainText("Hello World"));

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Break_Generates_Break()
    {
        const string expected = "<break />";

        var actual = new Break();

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Break_Generates_Time_Attribute()
    {
        const string expected = @"<break time=""3s"" />";

        var actual = new Break { Time = "3s" };

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Break_Generates_Strength()
    {
        const string expected = @"<break strength=""x-weak"" />";

        var actual = new Break { Strength = BreakStrength.ExtraWeak };

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Sayas_Generates_Sayas()
    {
        const string expected = @"<say-as interpret-as=""spell-out"">Hello World</say-as>";

        var actual = new SayAs("Hello World", InterpretAs.SpellOut);

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Sayas_Generates_Format()
    {
        const string expected = @"<say-as interpret-as=""spell-out"" format=""ymd"">Hello World</say-as>";

        var actual = new SayAs("Hello World", InterpretAs.SpellOut) { Format = "ymd" };

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Word_Generates_w()
    {
        const string expected = @"<w role=""amazon:VB"">world</w>";

        var actual = new Word("world", WordRole.Verb);

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Sub_generates_sub()
    {
        const string expected = @"<sub alias=""magnesium"">Mg</sub>";

        var actual = new Sub("Mg", "magnesium");

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Prosody_generates_prosody()
    {
        const string expected = @"<prosody rate=""150%"" pitch=""x-low"" volume=""-5dB"">Hello World</prosody>";

        var actual = new Prosody
        {
            Rate = ProsodyRate.Percent(150),
            Pitch = ProsodyPitch.ExtraLow,
            Volume = ProsodyVolume.Decibel(-5)
        };

        actual.Elements.Add(new PlainText("Hello World"));

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Emphasis_generates_emphasis()
    {
        const string expected = @"<emphasis level=""strong"">Hello World</emphasis>";

        var actual = new Emphasis("Hello World");
        actual.Level = EmphasisLevel.Strong;

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Phoneme_generates_phoneme()
    {
        const string expected = @"<phoneme alphabet=""ipa"" ph=""pɪˈkɑːn"">pecan</phoneme>";

        var actual = new Phoneme("pecan", PhonemeAlphabet.International, "pɪˈkɑːn");

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Audio_generate_audio()
    {
        const string expected = @"<audio src=""http://example.com/example.mp3"">Hello World</audio>";

        var actual = new Audio("http://example.com/example.mp3");
        actual.Elements.Add(new PlainText("Hello World"));

        CompareXml(expected, actual);
    }

    [Fact]
    public void Ssml_Amazon_Effect_generate_amazon_effect()
    {
        const string expected = @"<speak><amazon:effect name=""whispered"">Hello World</amazon:effect></speak>";

        var effect = new AmazonEffect("Hello World");

        //Can't use Comparexml because this tag has meant a change to the speech element ToXml method
        var xmlHost = new SsmlSpeech();
        xmlHost.Elements.Add(effect);
        var actual = xmlHost.ToXml();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TerseSsml_Produces_Identical_Xml()
    {
        const string speech1 = "Welcome to";
        const string speech2 = "the most awesome game ever";
        const string speech3 = "what do you want to do?";

        var expected = new SsmlSpeech
        {
            Elements = new List<ISsml>
            {
                new Paragraph
                {
                    Elements = new List<IParagraphSsml>
                    {
                        new PlainText(speech1),
                        new Prosody
                        {
                            Rate = ProsodyRate.Fast,
                            Elements = new List<ISsml> {new Sentence(speech2)}
                        },
                        new Sentence(speech3)
                    }
                }
            }
        };

        var actual = new SsmlSpeech(
            new Paragraph(
                new PlainText(speech1),
                new Prosody(new Sentence(speech2)) { Rate = ProsodyRate.Fast },
                new Sentence(speech3)
            ));

        Assert.Equal(expected.ToXml(), actual.ToXml());
    }

    [Fact]
    public void Ssml_VoiceAndLang_GenerateCorrectly()
    {
        var expected = "<voice name=\"Celine\"><lang xml:lang=\"fr-FR\">Je ne parle pas francais</lang></voice>";
        var speech =
            new Voice("Celine",
                new Lang("fr-FR", new PlainText("Je ne parle pas francais"))
            );
        CompareXml(expected, speech);
    }

    [Fact]
    public void Ssml_Alexa_Name_generate_alexa_name()
    {
        const string expected = "<speak><alexa:name type=\"first\" personId=\"amzn1.ask.person.ABCDEF\" /></speak>";

        var alexaName = new AlexaName("amzn1.ask.person.ABCDEF");

        var xmlHost = new SsmlSpeech();
        xmlHost.Elements.Add(alexaName);
        var actual = xmlHost.ToXml();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Ssml_AmazonDomain_generate_domain()
    {
        const string expected = @"<speak><amazon:domain name=""news"">A miniature manuscript</amazon:domain></speak>";

        var xmlHost = new SsmlSpeech();
        var actual = new AmazonDomain(DomainName.News);
        actual.Elements.Add(new PlainText("A miniature manuscript"));
        xmlHost.Elements.Add(actual);
        Assert.Equal(expected, xmlHost.ToXml());
    }

    [Fact]
    public void Ssml_AmazonEmotion_generate_emotion()
    {
        const string expected = @"<speak><amazon:emotion name=""excited"" intensity=""medium"">Christina wins this round!</amazon:emotion></speak>";

        var xmlHost = new SsmlSpeech();
        var actual = new AmazonEmotion(EmotionName.Excited, EmotionIntensity.Medium);
        actual.Elements.Add(new PlainText("Christina wins this round!"));
        xmlHost.Elements.Add(actual);
        Assert.Equal(expected,xmlHost.ToXml());
    }

    // AutoFixture-based tests
    [Theory]
    [ModelAutoData]
    public void Speech_WithGeneratedData_GeneratesValidXml(SsmlSpeech speech)
    {
        speech.Elements.Should().NotBeEmpty();
        
        var xml = speech.ToXml();
        xml.Should().NotBeNullOrEmpty();
        xml.Should().StartWith("<speak");
        xml.Should().EndWith("</speak>");
    }

    [Theory]
    [ModelAutoData]
    public void PlainText_WithGeneratedData_HasValidContent(PlainText plainText)
    {
        var xml = plainText.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().NotBeNullOrEmpty();
        xml.Should().NotContain("<");
        xml.Should().NotContain(">");
        
    }

    [Theory]
    [ModelAutoData]
    public void Break_WithGeneratedData_GeneratesValidXml(Break breakElement)
    {
        var xml = breakElement.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<break");
        xml.Should().EndWith(" />");
        
    }

    [Theory]
    [ModelAutoData]
    public void SayAs_WithGeneratedData_HasValidAttributes(SayAs sayAs)
    {
        var xml = sayAs.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<say-as");
        xml.Should().Contain("interpret-as=");
        xml.Should().EndWith("</say-as>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Word_WithGeneratedData_HasValidRole(Word word)
    {
        var xml = word.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<w");
        xml.Should().Contain("role=");
        xml.Should().EndWith("</w>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Sub_WithGeneratedData_HasValidAlias(Sub sub)
    {
        var xml = sub.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<sub");
        xml.Should().Contain("alias=");
        xml.Should().EndWith("</sub>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Prosody_WithGeneratedData_HasValidAttributes(Prosody prosody)
    {
        prosody.Elements.Should().NotBeEmpty();
        
        var xml = prosody.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<prosody");
        xml.Should().EndWith("</prosody>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Emphasis_WithGeneratedData_HasValidLevel(Emphasis emphasis)
    {
        var xml = emphasis.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<emphasis");
        xml.Should().Contain("level=");
        xml.Should().EndWith("</emphasis>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Phoneme_WithGeneratedData_HasValidAttributes(Phoneme phoneme)
    {
        var xml = phoneme.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<phoneme");
        xml.Should().Contain("alphabet=");
        xml.Should().Contain("ph=");
        xml.Should().EndWith("</phoneme>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Audio_WithGeneratedData_HasValidSrc(Audio audio)
    {
        var xml = audio.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<audio");
        xml.Should().Contain("src=");
        // Audio can end with either "</audio>" (with children) or "/>" (self-closing, no children)
        xml.Should().Match(x => x.EndsWith("</audio>") || x.EndsWith("/>"));
        
    }

    [Theory]
    [ModelAutoData]
    public void Voice_WithGeneratedData_HasValidName(Voice voice)
    {
        var xml = voice.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<voice");
        xml.Should().Contain("name=");
        xml.Should().EndWith("</voice>");
        
    }

    [Theory]
    [ModelAutoData]
    public void Lang_WithGeneratedData_HasValidLanguage(Lang lang)
    {
        var xml = lang.ToXml().ToString(SaveOptions.DisableFormatting);
        xml.Should().StartWith("<lang");
        xml.Should().Contain("xml:lang=");
        xml.Should().EndWith("</lang>");
        
    }

    [Theory]
    [ModelAutoData]
    public void AmazonEffect_WithGeneratedData_GeneratesValidXml(AmazonEffect effect)
    {
        var speech = new SsmlSpeech();
        speech.Elements.Add(effect);
        
        var xml = speech.ToXml();
        xml.Should().Contain("<amazon:effect");
        xml.Should().Contain("</amazon:effect>");
        
    }

    [Theory]
    [ModelAutoData]
    public void AmazonDomain_WithGeneratedData_HasValidDomain(AmazonDomain domain)
    {
        domain.Elements.Should().NotBeEmpty();
        
        var speech = new SsmlSpeech();
        speech.Elements.Add(domain);
        
        var xml = speech.ToXml();
        xml.Should().Contain("<amazon:domain");
        xml.Should().Contain("name=");
        xml.Should().Contain("</amazon:domain>");
        
    }

    [Theory]
    [ModelAutoData]
    public void AmazonEmotion_WithGeneratedData_HasValidAttributes(AmazonEmotion emotion)
    {
        emotion.Elements.Should().NotBeEmpty();
        
        var speech = new SsmlSpeech();
        speech.Elements.Add(emotion);
        
        var xml = speech.ToXml();
        xml.Should().Contain("<amazon:emotion");
        xml.Should().Contain("name=");
        xml.Should().Contain("intensity=");
        xml.Should().Contain("</amazon:emotion>");
        
    }

    [Theory]
    [ModelAutoData]
    public void AlexaName_WithGeneratedData_HasValidPersonId(AlexaName alexaName)
    {
        var speech = new SsmlSpeech();
        speech.Elements.Add(alexaName);
        
        var xml = speech.ToXml();
        xml.Should().Contain("<alexa:name");
        xml.Should().Contain("personId=");
        xml.Should().Contain("amzn1.ask.person.");
        
    }

    private void CompareXml(string expected, ISsml ssml)
    {
        var actual = ssml.ToXml().ToString(SaveOptions.DisableFormatting);
        Assert.Equal(expected, actual);
    }
}