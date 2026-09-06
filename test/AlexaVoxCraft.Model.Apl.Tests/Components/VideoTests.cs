using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class VideoTests : TestBase<VideoTests>
{
    [Fact]
    public async Task Video_Serializes()
    {
        var video = new Video
        {
            AudioTrack = "foreground",
            Autoplay = true,
            Muted = false,
            Scale = Scale.BestFill,
            Source = [new VideoSource("https://example.com/video.mp4")]
        };

        await TestHelper.VerifySerializedObject(video, AlexaJson, "Video");
    }
}
