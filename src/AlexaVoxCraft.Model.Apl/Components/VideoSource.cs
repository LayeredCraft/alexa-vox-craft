using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class VideoSource : IJsonSerializable<VideoSource>
{
    [JsonPropertyName("url")] public APLValue<Uri> Uri { get; set; } = null!;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Description { get; set; } = null!;

    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> DurationMilliseconds { get; set; } = null!;

    [JsonPropertyName("repeatCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> RepeatCount { get; set; } = null!;

    [JsonPropertyName("offset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Offset { get; set; } = null!;

    [JsonPropertyName("textTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<TextTrack> TextTrack { get; set; } = null!;

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
        Uri = ((APLValue<Uri>?)new Uri(url))!;
    }

    public static void RegisterTypeInfo<T>() where T : VideoSource
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var textTrackProp = info.Properties.FirstOrDefault(p => p.Name == "textTrack");
            textTrackProp?.CustomConverter = new APLValueCollectionConverter<TextTrack>(true);
        });
    }
}