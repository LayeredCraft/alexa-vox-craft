using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaHeadline : ResponsiveTemplate, IJsonSerializable<AlexaHeadline>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaHeadline);

    [JsonPropertyName("colorOverlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ColorOverlay { get; set; }


    [JsonPropertyName("overlayGradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? OverlayGradient { get; set; }


    [JsonPropertyName("videoAudioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? VideoAudioTrack { get; set; }


    [JsonPropertyName("videoAutoPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? VideoAutoPlay { get; set; }

    [JsonPropertyName("primaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PrimaryText { get; set; }

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryText { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    [JsonPropertyName("headerAttributionOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? HeaderAttributionOpacity { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaHeadline
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
    }
}