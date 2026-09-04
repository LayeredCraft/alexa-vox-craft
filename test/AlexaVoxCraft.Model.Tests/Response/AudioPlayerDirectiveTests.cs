using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for the AudioPlayer interface directives. This is a legacy-only
/// interface unused by any skill this session has real captures for, so all data is synthetic.
/// Directives are response objects the skill only ever builds and sends, so they are tested via
/// serialize, not deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class AudioPlayerDirectiveTests() : TestBase<AudioPlayerDirectiveTests>
{
    [Fact]
    public async Task AudioPlayerPlayDirective_Serializes_WithoutMetadata()
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

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "AudioPlayerPlayDirective_WithoutMetadata");
    }

    [Fact]
    public async Task AudioPlayerPlayDirective_Serializes_WithMetadata()
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
                    Art = new AudioItemSources { Sources = [new AudioItemSource("https://url-of-the-album-art-image.png")] },
                    BackgroundImage = new AudioItemSources { Sources = [new AudioItemSource("https://url-of-the-background-image.png")] }
                }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "AudioPlayerPlayDirective_WithMetadata");
    }

    [Fact]
    public async Task ClearQueueDirective_Serializes()
    {
        var directive = new ClearQueueDirective { ClearBehavior = ClearBehavior.ClearAll };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "ClearQueueDirective");
    }

    [Fact]
    public async Task StopDirective_Serializes()
    {
        var directive = new StopDirective();

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "StopDirective");
    }
}
