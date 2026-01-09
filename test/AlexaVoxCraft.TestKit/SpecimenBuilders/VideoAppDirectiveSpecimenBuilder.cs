using AlexaVoxCraft.Model.Response.Directive;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public sealed class VideoAppDirectiveSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        return request switch
        {
            Type type when type == typeof(VideoAppDirective) => CreateVideoAppDirective(context),
            Type type when type == typeof(VideoItem) => CreateVideoItem(context),
            Type type when type == typeof(VideoItemMetadata) => CreateVideoItemMetadata(context),
            _ => new NoSpecimen()
        };
    }

    private static VideoAppDirective CreateVideoAppDirective(ISpecimenContext context)
    {
        return new VideoAppDirective
        {
            VideoItem = context.Create<VideoItem>()
        };
    }

    private static VideoItem CreateVideoItem(ISpecimenContext context)
    {
        var videoUrls = new[]
        {
            "https://example.com/videos/sample-video-1.mp4",
            "https://example.com/videos/sample-video-2.mp4",
            "https://cdn.example.com/content/movie.mp4",
            "https://example.com/streams/live-video.m3u8"
        };

        var url = videoUrls[context.Create<int>() % videoUrls.Length];

        var videoItem = new VideoItem(url);

        // Randomly add metadata (70% chance)
        if (context.Create<int>() % 10 < 7)
        {
            videoItem.Metadata = context.Create<VideoItemMetadata>();
        }

        return videoItem;
    }

    private static VideoItemMetadata CreateVideoItemMetadata(ISpecimenContext context)
    {
        var titles = new[]
        {
            "Sample Video Title",
            "Amazing Documentary",
            "Music Video Premiere",
            "Live Stream Event",
            "Educational Content"
        };

        var subtitles = new[]
        {
            "Sample Video Subtitle",
            "Featuring Expert Commentary",
            "Official Music Video",
            "Live from Studio",
            "Learn Something New"
        };

        var title = titles[context.Create<int>() % titles.Length];
        var subtitle = subtitles[context.Create<int>() % subtitles.Length];

        return new VideoItemMetadata
        {
            Title = title,
            Subtitle = subtitle
        };
    }
}