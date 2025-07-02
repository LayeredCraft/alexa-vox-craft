using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Tests.Infrastructure;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;

namespace AlexaVoxCraft.Model.Tests.Directives;

public sealed class AudioPlayerDirectiveTests
{
    [Fact]
    public void AudioPlayerGeneratesCorrectJson()
    {
        var directive = new AudioPlayerPlayDirective
        {
            PlayBehavior = PlayBehavior.Enqueue,
            AudioItem = new AudioItem
            {
                Stream = new AudioItemStream
                {
                    Url = "https://url-of-the-stream-to-play",
                    Token = "opaque token representing this stream",
                    ExpectedPreviousToken = "opaque token representing the previous stream"
                }
            }
        };
        Assert.True(Utility.CompareJson(directive, "AudioPlayerWithoutMetadata.json"));
    }

    [Fact]
    public void AudioPlayerWithMetadataGeneratesCorrectJson()
    {
        var directive = new AudioPlayerPlayDirective
        {
            PlayBehavior = PlayBehavior.Enqueue,
            AudioItem = new AudioItem
            {
                Stream = new AudioItemStream
                {
                    Url = "https://url-of-the-stream-to-play",
                    Token = "opaque token representing this stream",
                    ExpectedPreviousToken = "opaque token representing the previous stream"
                },
                Metadata = new AudioItemMetadata
                {
                    Title = "title of the track to display",
                    Subtitle = "subtitle of the track to display",
                    Art = new AudioItemSources
                    {
                        Sources = new[] { new AudioItemSource("https://url-of-the-album-art-image.png") }.ToList()
                    },
                    BackgroundImage = new AudioItemSources { Sources = new[] { new AudioItemSource("https://url-of-the-background-image.png") }.ToList() }
                }
            }
        };
        Assert.True(Utility.CompareJson(directive, "AudioPlayerWithMetadata.json"));
    }

    [Fact]
    public void AudioPlayerWithMetadataDeserializesCorrectly()
    {
        var audioPlayer = Utility.ExampleFileContent<AudioPlayerPlayDirective>("AudioPlayerWithMetadata.json");
        Assert.Equal("title of the track to display", audioPlayer.AudioItem.Metadata.Title);
        Assert.Equal("subtitle of the track to display", audioPlayer.AudioItem.Metadata.Subtitle);
        Assert.Single(audioPlayer.AudioItem.Metadata.Art.Sources);
        Assert.Single(audioPlayer.AudioItem.Metadata.BackgroundImage.Sources);
        Assert.Equal("https://url-of-the-album-art-image.png", audioPlayer.AudioItem.Metadata.Art.Sources.First().Url);
        Assert.Equal("https://url-of-the-background-image.png", audioPlayer.AudioItem.Metadata.BackgroundImage.Sources.First().Url);
    }

    [Fact]
    public void AudioPlayerIgnoresMetadataWhenNull()
    {
        var audioPlayer = Utility.ExampleFileContent<AudioPlayerPlayDirective>("AudioPlayerWithoutMetadata.json");
        Assert.Null(audioPlayer.AudioItem.Metadata);
        Assert.Equal("https://url-of-the-stream-to-play", audioPlayer.AudioItem.Stream.Url);
    }

    [Fact]
    public void DeserializesExampleAudioPlayerWithMetadataJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("AudioPlayerWithMetadata.json");

        Assert.Equal(typeof(AudioPlayerPlayDirective), deserialized.GetType());

        var directive = (AudioPlayerPlayDirective)deserialized;

        Assert.Equal("AudioPlayer.Play", directive.Type);
        Assert.Equal(PlayBehavior.Enqueue, directive.PlayBehavior);

        var stream = directive.AudioItem.Stream;

        Assert.Equal("https://url-of-the-stream-to-play", stream.Url);
        Assert.Equal("opaque token representing this stream", stream.Token);
        Assert.Equal("opaque token representing the previous stream", stream.ExpectedPreviousToken);
        Assert.Equal(0, stream.OffsetInMilliseconds);

        var metadata = directive.AudioItem.Metadata;

        Assert.Equal("title of the track to display", metadata.Title);
        Assert.Equal("subtitle of the track to display", metadata.Subtitle);
        Assert.Single(metadata.Art.Sources);
        Assert.Equal("https://url-of-the-album-art-image.png", metadata.Art.Sources.First().Url);
        Assert.Single(metadata.BackgroundImage.Sources);
        Assert.Equal("https://url-of-the-background-image.png", metadata.BackgroundImage.Sources.First().Url);
    }

    [Fact]
    public void DeserializesExampleClearQueueDirectiveJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("ClearQueueDirective.json");

        Assert.Equal(typeof(ClearQueueDirective), deserialized.GetType());

        var directive = (ClearQueueDirective)deserialized;

        Assert.Equal("AudioPlayer.ClearQueue", directive.Type);
        Assert.Equal(ClearBehavior.ClearAll, directive.ClearBehavior);
    }

    [Fact]
    public void DeserializesExampleStopDirectiveJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("StopDirective.json");

        Assert.Equal(typeof(StopDirective), deserialized.GetType());

        var directive = (StopDirective)deserialized;

        Assert.Equal("AudioPlayer.Stop", directive.Type);
    }

    // AutoFixture-based tests
    [Theory]
    [ModelAutoData]
    public void AudioPlayerPlayDirective_WithGeneratedData_HasValidProperties(AudioPlayerPlayDirective directive)
    {
        directive.Type.Should().Be("AudioPlayer.Play");
        directive.PlayBehavior.Should().BeOneOf(PlayBehavior.ReplaceAll, PlayBehavior.Enqueue, PlayBehavior.ReplaceEnqueued);
        directive.AudioItem.Should().NotBeNull();
        directive.AudioItem.Stream.Should().NotBeNull();
        directive.AudioItem.Stream.Url.Should().NotBeNullOrEmpty();
        directive.AudioItem.Stream.Token.Should().NotBeNullOrEmpty();
        directive.AudioItem.Stream.OffsetInMilliseconds.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [ModelAutoData]
    public void ClearQueueDirective_WithGeneratedData_HasValidProperties(ClearQueueDirective directive)
    {
        directive.Type.Should().Be("AudioPlayer.ClearQueue");
        directive.ClearBehavior.Should().BeOneOf(ClearBehavior.ClearAll, ClearBehavior.ClearEnqueued);
    }

    [Theory]
    [ModelAutoData]
    public void StopDirective_WithGeneratedData_HasValidProperties(StopDirective directive)
    {
        directive.Type.Should().Be("AudioPlayer.Stop");
    }

    [Theory]
    [ModelAutoData]
    public void AudioItem_WithGeneratedData_HasValidStream(AudioItem audioItem)
    {
        audioItem.Stream.Should().NotBeNull();
        audioItem.Stream.Url.Should().NotBeNullOrEmpty();
        audioItem.Stream.Token.Should().NotBeNullOrEmpty();
        audioItem.Stream.OffsetInMilliseconds.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [ModelAutoData]
    public void AudioItemStream_WithGeneratedData_HasValidProperties(AudioItemStream stream)
    {
        stream.Url.Should().NotBeNullOrEmpty();
        stream.Token.Should().NotBeNullOrEmpty();
        stream.OffsetInMilliseconds.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [ModelAutoData]
    public void AudioItemMetadata_WithGeneratedData_HasValidProperties(AudioItemMetadata metadata)
    {
        metadata.Title.Should().NotBeNullOrEmpty();
        metadata.Subtitle.Should().NotBeNullOrEmpty();
        metadata.Art.Should().NotBeNull();
        metadata.BackgroundImage.Should().NotBeNull();
        metadata.Art.Sources.Should().NotBeEmpty();
        metadata.BackgroundImage.Sources.Should().NotBeEmpty();
    }

    [Theory]
    [ModelAutoData]
    public void AudioItemSources_WithGeneratedData_HasValidSources(AudioItemSources sources)
    {
        sources.Sources.Should().NotBeEmpty();
        sources.Sources.Should().HaveCountLessThanOrEqualTo(3);
        sources.Sources.Should().OnlyContain(source => !string.IsNullOrEmpty(source.Url));
    }

    [Theory]
    [ModelAutoData]
    public void AudioItemSource_WithGeneratedData_HasValidUrl(AudioItemSource source)
    {
        source.Url.Should().NotBeNullOrEmpty();
        source.Url.Should().StartWith("https://");
    }
}