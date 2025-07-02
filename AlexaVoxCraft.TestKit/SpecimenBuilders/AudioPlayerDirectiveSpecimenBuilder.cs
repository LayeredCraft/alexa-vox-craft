using AlexaVoxCraft.Model.Response.Directive;
using AutoFixture;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public sealed class AudioPlayerDirectiveSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        return request switch
        {
            Type type when type == typeof(AudioPlayerPlayDirective) => CreateAudioPlayerPlayDirective(context),
            Type type when type == typeof(ClearQueueDirective) => CreateClearQueueDirective(context),
            Type type when type == typeof(StopDirective) => CreateStopDirective(context),
            Type type when type == typeof(AudioItem) => CreateAudioItem(context),
            Type type when type == typeof(AudioItemStream) => CreateAudioItemStream(context),
            Type type when type == typeof(AudioItemMetadata) => CreateAudioItemMetadata(context),
            Type type when type == typeof(AudioItemSources) => CreateAudioItemSources(context),
            Type type when type == typeof(AudioItemSource) => CreateAudioItemSource(context),
            _ => new NoSpecimen()
        };
    }

    private static AudioPlayerPlayDirective CreateAudioPlayerPlayDirective(ISpecimenContext context)
    {
        var playBehaviors = new[] { PlayBehavior.ReplaceAll, PlayBehavior.Enqueue, PlayBehavior.ReplaceEnqueued };
        var playBehavior = playBehaviors[context.Create<int>() % playBehaviors.Length];

        return new AudioPlayerPlayDirective
        {
            PlayBehavior = playBehavior,
            AudioItem = context.Create<AudioItem>()
        };
    }

    private static ClearQueueDirective CreateClearQueueDirective(ISpecimenContext context)
    {
        var clearBehaviors = new[] { ClearBehavior.ClearAll, ClearBehavior.ClearEnqueued };
        var clearBehavior = clearBehaviors[context.Create<int>() % clearBehaviors.Length];

        return new ClearQueueDirective
        {
            ClearBehavior = clearBehavior
        };
    }

    private static StopDirective CreateStopDirective(ISpecimenContext context)
    {
        return new StopDirective();
    }

    private static AudioItem CreateAudioItem(ISpecimenContext context)
    {
        var audioItem = new AudioItem
        {
            Stream = context.Create<AudioItemStream>()
        };

        // Randomly add metadata (50% chance)
        if (context.Create<bool>())
        {
            audioItem.Metadata = context.Create<AudioItemMetadata>();
        }

        return audioItem;
    }

    private static AudioItemStream CreateAudioItemStream(ISpecimenContext context)
    {
        var urls = new[]
        {
            "https://example.com/audio/track1.mp3",
            "https://example.com/audio/track2.wav",
            "https://example.com/audio/track3.m4a",
            "https://cdn.example.com/streams/podcast.mp3"
        };

        var tokens = new[]
        {
            "token_12345",
            "stream_token_abcdef",
            "audio_session_xyz789",
            "playback_token_qwerty"
        };

        var previousTokens = new[]
        {
            "prev_token_98765",
            "previous_stream_fedcba",
            "last_audio_987zyx",
            null // Sometimes no previous token
        };

        var url = urls[context.Create<int>() % urls.Length];
        var token = tokens[context.Create<int>() % tokens.Length];
        var previousToken = previousTokens[context.Create<int>() % previousTokens.Length];
        var offset = context.Create<int>() % 60000; // 0-60 seconds in milliseconds

        return new AudioItemStream
        {
            Url = url,
            Token = token,
            ExpectedPreviousToken = previousToken,
            OffsetInMilliseconds = offset
        };
    }

    private static AudioItemMetadata CreateAudioItemMetadata(ISpecimenContext context)
    {
        var titles = new[]
        {
            "Amazing Song Title",
            "Podcast Episode #42",
            "Classical Symphony No. 5",
            "Rock Anthem 2024"
        };

        var subtitles = new[]
        {
            "Artist Name",
            "Featuring Guest Speaker",
            "Performed by Orchestra",
            "From the Latest Album"
        };

        var title = titles[context.Create<int>() % titles.Length];
        var subtitle = subtitles[context.Create<int>() % subtitles.Length];

        return new AudioItemMetadata
        {
            Title = title,
            Subtitle = subtitle,
            Art = context.Create<AudioItemSources>(),
            BackgroundImage = context.Create<AudioItemSources>()
        };
    }

    private static AudioItemSources CreateAudioItemSources(ISpecimenContext context)
    {
        var sourceCount = context.Create<int>() % 3 + 1; // 1-3 sources
        var sources = new List<AudioItemSource>();

        for (int i = 0; i < sourceCount; i++)
        {
            sources.Add(context.Create<AudioItemSource>());
        }

        return new AudioItemSources
        {
            Sources = sources
        };
    }

    private static AudioItemSource CreateAudioItemSource(ISpecimenContext context)
    {
        var imageUrls = new[]
        {
            "https://example.com/images/album-art-small.png",
            "https://example.com/images/album-art-large.jpg",
            "https://cdn.example.com/artwork/cover.png",
            "https://example.com/images/background.jpg"
        };

        var url = imageUrls[context.Create<int>() % imageUrls.Length];

        return new AudioItemSource(url);
    }
}