using AlexaVoxCraft.Model.Response.Ssml;
using AutoFixture.Kernel;
using DomainName = AlexaVoxCraft.Model.Response.Ssml.DomainName;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public sealed class SsmlSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        return request switch
        {
            Type type when type == typeof(Speech) => CreateSpeech(context),
            Type type when type == typeof(PlainText) => CreatePlainText(context),
            Type type when type == typeof(Sentence) => CreateSentence(context),
            Type type when type == typeof(Paragraph) => CreateParagraph(context),
            Type type when type == typeof(Break) => CreateBreak(context),
            Type type when type == typeof(SayAs) => CreateSayAs(context),
            Type type when type == typeof(Word) => CreateWord(context),
            Type type when type == typeof(Sub) => CreateSub(context),
            Type type when type == typeof(Prosody) => CreateProsody(context),
            Type type when type == typeof(Emphasis) => CreateEmphasis(context),
            Type type when type == typeof(Phoneme) => CreatePhoneme(context),
            Type type when type == typeof(Audio) => CreateAudio(context),
            Type type when type == typeof(Voice) => CreateVoice(context),
            Type type when type == typeof(Lang) => CreateLang(context),
            Type type when type == typeof(AmazonEffect) => CreateAmazonEffect(context),
            Type type when type == typeof(AmazonDomain) => CreateAmazonDomain(context),
            Type type when type == typeof(AmazonEmotion) => CreateAmazonEmotion(context),
            Type type when type == typeof(AlexaName) => CreateAlexaName(context),
            _ => new NoSpecimen()
        };
    }
    
    private static Speech CreateSpeech(ISpecimenContext context)
    {
        var speech = new Speech();
        var elementCount = context.Create<int>() % 3 + 1; // 1-3 elements
        
        for (int i = 0; i < elementCount; i++)
        {
            speech.Elements.Add(CreateRandomSsmlElement(context));
        }
        
        return speech;
    }
    
    private static ISsml CreateRandomSsmlElement(ISpecimenContext context)
    {
        var elementTypes = new[]
        {
            typeof(PlainText), typeof(Sentence), typeof(Paragraph), typeof(Break)
        };
        
        var elementType = elementTypes[context.Create<int>() % elementTypes.Length];
        return (ISsml)Create(elementType, context);
    }
    
    private static object Create(Type type, ISpecimenContext context)
    {
        var builder = new SsmlSpecimenBuilder();
        return builder.Create(type, context);
    }
    
    private static PlainText CreatePlainText(ISpecimenContext context)
    {
        var texts = new[]
        {
            "Hello World", "Welcome to our service", "Thank you for using our skill",
            "Have a great day", "Please try again later"
        };
        
        var text = texts[context.Create<int>() % texts.Length];
        return new PlainText(text);
    }
    
    private static Sentence CreateSentence(ISpecimenContext context)
    {
        var sentences = new[]
        {
            "This is a sample sentence.", "Welcome to the application.",
            "Please choose an option.", "Thank you for your time."
        };
        
        var text = sentences[context.Create<int>() % sentences.Length];
        return new Sentence(text);
    }
    
    private static Paragraph CreateParagraph(ISpecimenContext context)
    {
        var paragraph = new Paragraph();
        var elementCount = context.Create<int>() % 3 + 1; // 1-3 elements
        
        for (int i = 0; i < elementCount; i++)
        {
            paragraph.Elements.Add(CreatePlainText(context));
        }
        
        return paragraph;
    }
    
    private static Break CreateBreak(ISpecimenContext context)
    {
        var breakElement = new Break();
        
        var useTime = context.Create<bool>();
        if (useTime)
        {
            var times = new[] { "1s", "2s", "3s", "500ms", "1000ms" };
            breakElement.Time = times[context.Create<int>() % times.Length];
        }
        else
        {
            var strengths = new[]
            {
                BreakStrength.None, BreakStrength.ExtraWeak, BreakStrength.Weak,
                BreakStrength.Medium, BreakStrength.Strong, BreakStrength.ExtraStrong
            };
            breakElement.Strength = strengths[context.Create<int>() % strengths.Length];
        }
        
        return breakElement;
    }
    
    private static SayAs CreateSayAs(ISpecimenContext context)
    {
        var texts = new[] { "12345", "spell-out-text", "2024-01-15", "3.14159" };
        var text = texts[context.Create<int>() % texts.Length];
        
        var interpretAs = new[]
        {
            InterpretAs.SpellOut, InterpretAs.Number, InterpretAs.Ordinal, 
            InterpretAs.Digits, InterpretAs.Date, InterpretAs.Time
        };
        var interpret = interpretAs[context.Create<int>() % interpretAs.Length];
        
        var sayAs = new SayAs(text, interpret);
        
        if (context.Create<bool>())
        {
            var formats = new[] { "ymd", "mdy", "dmy", "my" };
            sayAs.Format = formats[context.Create<int>() % formats.Length];
        }
        
        return sayAs;
    }
    
    private static Word CreateWord(ISpecimenContext context)
    {
        var words = new[] { "world", "application", "service", "skill" };
        var word = words[context.Create<int>() % words.Length];
        
        var roles = new[]
        {
            WordRole.Verb, WordRole.PastParticiple, WordRole.Noun, WordRole.NonDefault
        };
        var role = roles[context.Create<int>() % roles.Length];
        
        return new Word(word, role);
    }
    
    private static Sub CreateSub(ISpecimenContext context)
    {
        var substitutions = new Dictionary<string, string>
        {
            { "Mg", "magnesium" },
            { "Al", "aluminum" },
            { "Fe", "iron" },
            { "Cu", "copper" }
        };
        
        var kvp = substitutions.ElementAt(context.Create<int>() % substitutions.Count);
        return new Sub(kvp.Key, kvp.Value);
    }
    
    private static Prosody CreateProsody(ISpecimenContext context)
    {
        var prosody = new Prosody();
        
        if (context.Create<bool>())
        {
            var rates = new[]
            {
                ProsodyRate.ExtraSlow, ProsodyRate.Slow, ProsodyRate.Medium,
                ProsodyRate.Fast, ProsodyRate.ExtraFast
            };
            prosody.Rate = rates[context.Create<int>() % rates.Length];
        }
        
        if (context.Create<bool>())
        {
            var pitches = new[]
            {
                ProsodyPitch.ExtraLow, ProsodyPitch.Low, ProsodyPitch.Medium,
                ProsodyPitch.High, ProsodyPitch.ExtraHigh
            };
            prosody.Pitch = pitches[context.Create<int>() % pitches.Length];
        }
        
        if (context.Create<bool>())
        {
            var volumes = new[]
            {
                ProsodyVolume.Silent, ProsodyVolume.ExtraSoft, ProsodyVolume.Soft,
                ProsodyVolume.Medium, ProsodyVolume.Loud, ProsodyVolume.ExtraLoud
            };
            prosody.Volume = volumes[context.Create<int>() % volumes.Length];
        }
        
        prosody.Elements.Add(CreatePlainText(context));
        return prosody;
    }
    
    private static Emphasis CreateEmphasis(ISpecimenContext context)
    {
        var texts = new[] { "Hello World", "Important message", "Pay attention" };
        var text = texts[context.Create<int>() % texts.Length];
        
        var emphasis = new Emphasis(text);
        
        var levels = new[]
        {
            EmphasisLevel.Strong, EmphasisLevel.Moderate, EmphasisLevel.Reduced
        };
        emphasis.Level = levels[context.Create<int>() % levels.Length];
        
        return emphasis;
    }
    
    private static Phoneme CreatePhoneme(ISpecimenContext context)
    {
        var words = new[] { "pecan", "tomato", "route", "caramel" };
        var pronunciations = new[] { "pɪˈkɑːn", "təˈmeɪtoʊ", "ruːt", "ˈkærəmɛl" };
        
        var index = context.Create<int>() % words.Length;
        var word = words[index];
        var pronunciation = pronunciations[index];
        
        var alphabets = new[]
        {
            PhonemeAlphabet.International, PhonemeAlphabet.SpeechAssessmentMethods
        };
        var alphabet = alphabets[context.Create<int>() % alphabets.Length];
        
        return new Phoneme(word, alphabet, pronunciation);
    }
    
    private static Audio CreateAudio(ISpecimenContext context)
    {
        var urls = new[]
        {
            "https://example.com/audio1.mp3",
            "https://example.com/audio2.wav",
            "https://example.com/audio3.ogg"
        };
        
        var url = urls[context.Create<int>() % urls.Length];
        var audio = new Audio(url);
        
        if (context.Create<bool>())
        {
            audio.Elements.Add(CreatePlainText(context));
        }
        
        return audio;
    }
    
    private static Voice CreateVoice(ISpecimenContext context)
    {
        var voices = new[] { "Joanna", "Matthew", "Ivy", "Justin", "Kendra" };
        var voice = voices[context.Create<int>() % voices.Length];
        
        return new Voice(voice, CreatePlainText(context));
    }
    
    private static Lang CreateLang(ISpecimenContext context)
    {
        var languages = new[] { "en-US", "en-GB", "fr-FR", "de-DE", "es-ES" };
        var lang = languages[context.Create<int>() % languages.Length];
        
        return new Lang(lang, CreatePlainText(context));
    }
    
    private static AmazonEffect CreateAmazonEffect(ISpecimenContext context)
    {
        var texts = new[] { "Hello World", "This is whispered", "Special effect" };
        var text = texts[context.Create<int>() % texts.Length];
        
        return new AmazonEffect(text);
    }
    
    private static AmazonDomain CreateAmazonDomain(ISpecimenContext context)
    {
        var domains = new[]
        {
            DomainName.News, DomainName.Music, DomainName.Conversational
        };
        var domain = domains[context.Create<int>() % domains.Length];
        
        var amazonDomain = new AmazonDomain(domain);
        amazonDomain.Elements.Add(CreatePlainText(context));
        
        return amazonDomain;
    }
    
    private static AmazonEmotion CreateAmazonEmotion(ISpecimenContext context)
    {
        var emotions = new[]
        {
            EmotionName.Excited, EmotionName.Disappointed
        };
        var emotion = emotions[context.Create<int>() % emotions.Length];
        
        var intensities = new[]
        {
            EmotionIntensity.Low, EmotionIntensity.Medium, EmotionIntensity.High
        };
        var intensity = intensities[context.Create<int>() % intensities.Length];
        
        var amazonEmotion = new AmazonEmotion(emotion, intensity);
        amazonEmotion.Elements.Add(CreatePlainText(context));
        
        return amazonEmotion;
    }
    
    private static AlexaName CreateAlexaName(ISpecimenContext context)
    {
        var personIds = new[]
        {
            "amzn1.ask.person.ABCDEF",
            "amzn1.ask.person.123456",
            "amzn1.ask.person.GHIJKL"
        };
        
        var personId = personIds[context.Create<int>() % personIds.Length];
        return new AlexaName(personId);
    }
}