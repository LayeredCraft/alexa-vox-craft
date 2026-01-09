using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class VideoSource : IJsonSerializable<VideoSource>
{
    [JsonPropertyName("url")] public APLValue<Uri> Uri { get; set; }

    [JsonPropertyName("description")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Description { get; set; }

    [JsonPropertyName("duration")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> DurationMilliseconds { get; set; }

    [JsonPropertyName("repeatCount")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> RepeatCount { get; set; }

    [JsonPropertyName("offset")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Offset { get; set; }

    [JsonPropertyName("textTrack")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<TextTrack>> TextTrack { get; set; }

    public static List<VideoSource> FromUrl(string url)
    {
        return new List<VideoSource> { new VideoSource(url) };
    }

    public List<VideoSource> FromUrl(IEnumerable<string> urls)
    {
        return urls.Select(u => new VideoSource(u)).ToList();
    }

    public VideoSource()
    {
    }

    public VideoSource(string url)
    {
        Uri = new Uri(url);
    }

    public static void RegisterTypeInfo<T>() where T : VideoSource
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var textTrackProp = info.Properties.FirstOrDefault(p => p.Name == "textTrack");
            if (textTrackProp is not null)
            {
                textTrackProp.CustomConverter = new GenericSingleOrListConverter<TextTrack>(true);
            }
        });
    }
}