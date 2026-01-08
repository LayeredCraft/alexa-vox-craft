using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Frame : APLComponent, IJsonSerializable<Frame>
{
    public Frame()
    {
    }

    public Frame(APLComponent item)
    {
        Item = [item];
    }

    public Frame(params APLComponent[] item) : this((IEnumerable<APLComponent>)item)
    {
    }

    public Frame(IEnumerable<APLComponent> item)
    {
        Item = new APLCollection<APLComponent>(item);
    }


    public const string ComponentType = "Frame";

    [JsonPropertyName("type")] public override string Type => ComponentType;

    [JsonPropertyName("background")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<APLGradient>? Background { get; set; }

    [JsonPropertyName("backgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundColor { get; set; }

    [JsonPropertyName("borderBottomLeftRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? BorderBottomLeftRadius { get; set; }

    [JsonPropertyName("borderBottomRightRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? BorderBottomRightRadius { get; set; }

    [JsonPropertyName("borderColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BorderColor { get; set; }

    [JsonPropertyName("borderRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? BorderRadius { get; set; }

    [JsonPropertyName("borderTopLeftRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? BorderTopLeftRadius { get; set; }

    [JsonPropertyName("borderTopRightRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? BorderTopRightRadius { get; set; }

    [JsonPropertyName("borderWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? BorderWidth { get; set; }

    [JsonPropertyName("item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLCollection<APLComponent>? Item { get; set; }

    [JsonPropertyName("borderStrokeWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLAbsoluteDimensionValue? BorderStrokeWidth { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Frame
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}