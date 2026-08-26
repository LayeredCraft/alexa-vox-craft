using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;
using Compono;
using DomainName = AlexaVoxCraft.Model.Response.Ssml.DomainName;
using Audio = AlexaVoxCraft.Model.Response.Ssml.Audio;
using SpeechElement = AlexaVoxCraft.Model.Response.Ssml.Speech;

namespace AlexaVoxCraft.Model.Tests.TestKit;

public sealed class ModelLegacyProfile : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        builder.UseBogus();
        builder.For<SpeechElement>().UseConstructor();
        builder.For<Prosody>().UseConstructor();
        builder.For<Audio>().UseConstructor<string>();
        builder.For<Voice>().UseConstructor<string>();
        builder.For<Lang>().UseConstructor<string>();
        builder.For<AmazonDomain>().UseConstructor<string>();
        builder.For<AmazonEmotion>().UseConstructor<string, string>();
        builder.For<AlexaName>().UseConstructor();
        builder.For<AudioItemSource>().UseConstructor<string>();
        builder.For<VideoAppDirective>().UseConstructor();

        builder
            .Register<Intent>(CreateIntent)
            .Register<Slot>(() => CreateSlot("ZodiacSign"))
            .Register<SlotType>(CreateSlotType)
            .Register<SlotTypeValue>(CreateSlotTypeValue)
            .Register<SlotTypeValueName>(() => new SlotTypeValueName { Value = "Seattle", Synonyms = ["Sea-Tac"] })
            .Register<DialogDelegate>(() => new DialogDelegate { UpdatedIntent = CreateIntent() })
            .Register<DialogElicitSlot>(() => new DialogElicitSlot("ZodiacSign") { UpdatedIntent = CreateIntent() })
            .Register<DialogConfirmSlot>(() => new DialogConfirmSlot("Date") { UpdatedIntent = CreateIntent() })
            .Register<DialogConfirmIntent>(() => new DialogConfirmIntent { UpdatedIntent = CreateIntent() })
            .Register<DialogUpdateDynamicEntities>(() => new DialogUpdateDynamicEntities { UpdateBehavior = UpdateBehavior.Replace, Types = [CreateSlotType()] })
            .Register<SimpleCard>(() => new SimpleCard { Title = "Welcome", Content = "Thanks for using our skill." })
            .Register<StandardCard>(() => new StandardCard { Title = "Welcome", Content = "Thanks for using our skill.", Image = CreateCardImage() })
            .Register<AskForPermissionsConsentCard>(() =>
            {
                var card = new AskForPermissionsConsentCard();
                card.Permissions.Add(RequestedPermission.ReadHouseholdList);
                return card;
            })
            .Register<LinkAccountCard>(() => new LinkAccountCard())
            .Register<CardImage>(CreateCardImage)
            .Register<AudioPlayerPlayDirective>(() => new AudioPlayerPlayDirective { PlayBehavior = PlayBehavior.ReplaceAll, AudioItem = CreateAudioItem() })
            .Register<ClearQueueDirective>(() => new ClearQueueDirective { ClearBehavior = ClearBehavior.ClearAll })
            .Register<StopDirective>(() => new StopDirective())
            .Register<AudioItem>(CreateAudioItem)
            .Register<AudioItemStream>(() => new AudioItemStream { Url = "https://example.com/audio/track.mp3", Token = "token", OffsetInMilliseconds = 1000 })
            .Register<AudioItemMetadata>(() => new AudioItemMetadata { Title = "Track", Subtitle = "Artist", Art = CreateAudioItemSources(), BackgroundImage = CreateAudioItemSources() })
            .Register<AudioItemSources>(CreateAudioItemSources)
            .Register<AudioItemSource>(() => new AudioItemSource("https://example.com/images/art.png"))
            .Register<VideoAppDirective>(() => new VideoAppDirective { VideoItem = CreateVideoItem() })
            .Register<VideoItem>(CreateVideoItem)
            .Register<VideoItemMetadata>(() => new VideoItemMetadata { Title = "Sample Video", Subtitle = "Sample Subtitle" })
            .Register<SpeechElement>(() => new SpeechElement(new PlainText("Hello world")))
            .Register<PlainText>(() => new PlainText("Hello world"))
            .Register<Sentence>(() => new Sentence("Hello world."))
            .Register<Paragraph>(() =>
            {
                var paragraph = new Paragraph();
                paragraph.Elements.Add(new PlainText("Hello world"));
                return paragraph;
            })
            .Register<Break>(() => new Break { Strength = BreakStrength.Medium })
            .Register<SayAs>(() => new SayAs("12345", InterpretAs.Digits))
            .Register<Word>(() => new Word("hello", WordRole.Noun))
            .Register<Sub>(() => new Sub("Mg", "magnesium"))
            .Register<Prosody>(() =>
            {
                var prosody = new Prosody { Rate = ProsodyRate.Medium, Pitch = ProsodyPitch.Medium, Volume = ProsodyVolume.Medium };
                prosody.Elements.Add(new PlainText("Hello world"));
                return prosody;
            })
            .Register<Emphasis>(() => new Emphasis("important") { Level = EmphasisLevel.Strong })
            .Register<Phoneme>(() => new Phoneme("tomato", "ipa", "təˈmeɪtoʊ"))
            .Register<Audio>(() => new Audio("https://example.com/audio/track.mp3"))
            .Register<Voice>(() => new Voice("Joanna", new PlainText("Hello world")))
            .Register<Lang>(() => new Lang("en-US", new PlainText("Hello world")))
            .Register<AmazonEffect>(() => new AmazonEffect("Hello world"))
            .Register<AmazonDomain>(() => new AmazonDomain(DomainName.News, new PlainText("Hello world")))
            .Register<AmazonEmotion>(() => new AmazonEmotion(EmotionName.Excited, EmotionIntensity.Medium, new PlainText("Hello world")))
            .Register<AlexaName>(() => new AlexaName("amzn1.ask.person.test"));
    }

    private static Intent CreateIntent() => new()
    {
        Name = "GetZodiacHoroscopeIntent",
        ConfirmationStatus = ConfirmationStatus.None,
        Slots = new Dictionary<string, Slot> { ["ZodiacSign"] = CreateSlot("ZodiacSign") }
    };

    private static Slot CreateSlot(string name) => new()
    {
        Name = name,
        Value = "virgo",
        ConfirmationStatus = ConfirmationStatus.None
    };

    private static SlotType CreateSlotType() => new()
    {
        Name = "CitySlotType",
        Values = [CreateSlotTypeValue()]
    };

    private static SlotTypeValue CreateSlotTypeValue() => new()
    {
        Id = "SEA",
        Name = new SlotTypeValueName { Value = "Seattle", Synonyms = ["Sea-Tac"] }
    };

    private static CardImage CreateCardImage() => new()
    {
        SmallImageUrl = "https://example.com/small.png",
        LargeImageUrl = "https://example.com/large.png"
    };

    private static AudioItem CreateAudioItem() => new()
    {
        Stream = new AudioItemStream { Url = "https://example.com/audio/track.mp3", Token = "token", OffsetInMilliseconds = 1000 },
        Metadata = new AudioItemMetadata { Title = "Track", Subtitle = "Artist", Art = CreateAudioItemSources(), BackgroundImage = CreateAudioItemSources() }
    };

    private static AudioItemSources CreateAudioItemSources() => new()
    {
        Sources = [new AudioItemSource("https://example.com/images/art.png")]
    };

    private static VideoItem CreateVideoItem() => new("https://example.com/videos/sample.mp4")
    {
        Metadata = new VideoItemMetadata { Title = "Sample Video", Subtitle = "Sample Subtitle" }
    };
}
