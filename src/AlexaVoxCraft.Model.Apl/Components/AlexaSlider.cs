using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaSlider : AlexaSliderBase, IJsonSerializable<AlexaSlider>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaSlider);

    [JsonPropertyName("sliderId")] public APLValue<string>? SliderId { get; set; }

    [JsonPropertyName("metadataPosition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<MetadataPosition?>? MetadataPosition { get; set; }

    [JsonPropertyName("iconLeftGraphicSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? IconLeftGraphicSource { get; set; }

    [JsonPropertyName("iconRightGraphicSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? IconRightGraphicSource { get; set; }

    [JsonPropertyName("sliderSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SliderSize?>? SliderSize { get; set; }

    [JsonPropertyName("sliderType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SliderType?>? SliderType { get; set; }

    [JsonPropertyName("sliderMoveMillisecond")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? SliderMoveMillisecond { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaSlider
    {
        AlexaSliderBase.RegisterTypeInfo<T>();
    }
}