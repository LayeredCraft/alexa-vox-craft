using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Tests.Infrastructure;

namespace AlexaVoxCraft.Model.Tests.Directives;

public sealed class VideoAppDirectiveTests
{
    [Fact]
    public void Creates_VideoAppDirective()
    {
        var videoItem = new VideoItem("https://www.example.com/video/sample-video-1.mp4")
        {
            Metadata = new VideoItemMetadata
            {
                Title = "Title for Sample Video",
                Subtitle = "Secondary Title for Sample Video"
            }
        };
        var actual = new VideoAppDirective { VideoItem = videoItem };

        Assert.True(Utility.CompareJson(actual, "VideoAppDirectiveWithMetadata.json"));
    }

    [Fact]
    public void Create_VideoAppDirective_FromSource()
    {
        var actual = new VideoAppDirective("https://www.example.com/video/sample-video-1.mp4");
        Assert.True(Utility.CompareJson(actual, "VideoAppDirective.json"));
    }

    [Fact]
    public void DeserializesExampleDirectiveVideoAppDirectiveWithMetadata()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("VideoAppDirectiveWithMetadata.json");

        Assert.Equal(typeof(VideoAppDirective), deserialized.GetType());

        var directive = (VideoAppDirective)deserialized;

        Assert.Equal("VideoApp.Launch", directive.Type);
        Assert.Equal("https://www.example.com/video/sample-video-1.mp4", directive.VideoItem.Source);
        Assert.Equal("Title for Sample Video", directive.VideoItem.Metadata.Title);
        Assert.Equal("Secondary Title for Sample Video", directive.VideoItem.Metadata.Subtitle);
    }

    [Fact]
    public void DirectiveShouldEndSessionOverrideSupport()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        Assert.Null(tell.Response.ShouldEndSession);
    }

    [Fact]
    public void MultipleShouldEndDirectivesWithCommonRequirement()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        Assert.Null(tell.Response.ShouldEndSession);
    }

    [Fact]
    public void ContradictingEndSessionOverrideDefaultsToExplicit()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        //As VideoApp needs a null EndSession and FakeDirective needs false - reverts to explicit
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new FakeDirective());
        Assert.True(tell.Response.ShouldEndSession);
    }

    private class FakeDirective : IEndSessionDirective
    {
        public string Type => "fake";
        public bool? ShouldEndSession => false;
    }

    // AutoFixture-based tests
    [Theory]
    [ModelAutoData]
    public void VideoAppDirective_WithGeneratedData_HasValidProperties(VideoAppDirective directive)
    {
        directive.Type.Should().Be("VideoApp.Launch");
        directive.VideoItem.Should().NotBeNull();
        directive.VideoItem.Source.Should().NotBeNullOrEmpty();
        directive.VideoItem.Source.Should().StartWith("https://");
    }

    [Theory]
    [ModelAutoData]
    public void VideoItem_WithGeneratedData_HasValidSource(VideoItem videoItem)
    {
        videoItem.Source.Should().NotBeNullOrEmpty();
        videoItem.Source.Should().StartWith("https://");
    }

    [Theory]
    [ModelAutoData]
    public void VideoItemMetadata_WithGeneratedData_HasValidProperties(VideoItemMetadata metadata)
    {
        metadata.Title.Should().NotBeNullOrEmpty();
        metadata.Subtitle.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [ModelAutoData]
    public void VideoAppDirective_ShouldEndSessionBehavior_IsNull(VideoAppDirective directive)
    {
        directive.ShouldEndSession.Should().BeNull();
    }
}