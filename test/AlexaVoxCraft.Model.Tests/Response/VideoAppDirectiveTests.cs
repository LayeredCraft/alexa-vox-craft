using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for the VideoApp interface directive, plus the general
/// <see cref="IEndSessionDirective"/> override behavior it participates in. VideoApp is a
/// legacy-only interface unused by any skill this session has real captures for, so all data is
/// synthetic. Directives are response objects the skill only ever builds and sends, so they are
/// tested via serialize, not deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class VideoAppDirectiveTests() : TestBase<VideoAppDirectiveTests>
{
    [Fact]
    public async Task VideoAppDirective_Serializes_WithMetadata()
    {
        var directive = new VideoAppDirective
        {
            VideoItem = new VideoItem("https://www.example.com/video/sample-video-1.mp4")
            {
                Metadata = new VideoItemMetadata { Title = "Title for Sample Video", Subtitle = "Secondary Title for Sample Video" }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "VideoAppDirective_WithMetadata");
    }

    [Fact]
    public async Task VideoAppDirective_Serializes_FromSource()
    {
        var directive = new VideoAppDirective("https://www.example.com/video/sample-video-1.mp4");

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "VideoAppDirective_FromSource");
    }

    [Fact]
    public void ShouldEndSession_OverridesToNull_WhenDirectiveRequiresIt()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        tell.Response.ShouldEndSession.Should().BeTrue();

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));

        tell.Response.ShouldEndSession.Should().BeNull();
    }

    [Fact]
    public void ShouldEndSession_StaysNull_WithMultipleDirectivesAgreeing()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        tell.Response.ShouldEndSession.Should().BeTrue();

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));

        tell.Response.ShouldEndSession.Should().BeNull();
    }

    [Fact]
    public void ShouldEndSession_RevertsToExplicit_WhenDirectivesContradict()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        tell.Response.ShouldEndSession.Should().BeTrue();

        // VideoApp needs a null EndSession and FakeDirective needs false - contradicting overrides revert to the explicit value.
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new FakeDirective());

        tell.Response.ShouldEndSession.Should().BeTrue();
    }

    private sealed class FakeDirective : IEndSessionDirective
    {
        public string Type => "fake";
        public bool? ShouldEndSession => false;
    }
}
