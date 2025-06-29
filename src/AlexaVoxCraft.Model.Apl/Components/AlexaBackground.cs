using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaBackground : ResponsiveTemplate, IJsonSerializable<AlexaBackground>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaBackground);

    [JsonPropertyName("colorOverlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ColorOverlay { get; set; }


    [JsonPropertyName("overlayGradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? OverlayGradient { get; set; }


    [JsonPropertyName("videoAudioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? VideoAudioTrack { get; set; } = "foreground";


    [JsonPropertyName("videoAutoPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? VideoAutoPlay { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaBackground
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
    }
}