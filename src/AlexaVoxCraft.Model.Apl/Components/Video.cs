using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Video : APLComponent, IJsonSerializable<Video>
{
    [JsonPropertyName("type")]
    public override string Type => "Video";

    [JsonPropertyName("audioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AudioTrack { get; set; }

    [JsonPropertyName("autoplay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Autoplay { get; set; }

    [JsonPropertyName("muted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Muted { get; set; }

    [JsonPropertyName("scale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale>? Scale { get; set; }

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Align { get; set; }

    [JsonPropertyName("onEnd")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<APLCommand>>? OnEnd { get; set; }

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<VideoSource>>? Source { get; set; }

    [JsonPropertyName("onPause")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnPause { get; set; }

    [JsonPropertyName("onPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnPlay { get; set; }

    [JsonPropertyName("onTrackUpdate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnTrackUpdate { get; set; }

    [JsonPropertyName("onTrackReady")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnTrackReady { get; set; }

    [JsonPropertyName("onTrackFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnTrackFail { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Video
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var sourceProp = info.Properties.FirstOrDefault(p => p.Name == "source");
            if (sourceProp is not null)
            {
                sourceProp.CustomConverter = new GenericSingleOrListConverter<VideoSource>(true);
            }

            var onPauseProp = info.Properties.FirstOrDefault(p => p.Name == "onPause");
            if (onPauseProp is not null)
            {
                onPauseProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onPlayProp = info.Properties.FirstOrDefault(p => p.Name == "onPlay");
            if (onPlayProp is not null)
            {
                onPlayProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onTrackUpdateProp = info.Properties.FirstOrDefault(p => p.Name == "onTrackUpdate");
            if (onTrackUpdateProp is not null)
            {
                onTrackUpdateProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onTrackReadyProp = info.Properties.FirstOrDefault(p => p.Name == "onTrackReady");
            if (onTrackReadyProp is not null)
            {
                onTrackReadyProp.CustomConverter = new APLCommandListConverter(true);
            }

            var onTrackFailProp = info.Properties.FirstOrDefault(p => p.Name == "onTrackFail");
            if (onTrackFailProp is not null)
            {
                onTrackFailProp.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}