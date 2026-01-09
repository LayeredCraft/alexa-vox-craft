using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class VectorGraphic : TouchComponent, IJsonSerializable<VectorGraphic>
{
    [JsonPropertyName("type")] public override string Type => nameof(VectorGraphic);

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Align { get; set; }

    [JsonPropertyName("scale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale>? Scale { get; set; }

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Source { get; set; }

    [JsonPropertyName("onLoad")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnLoad { get; set; }

    [JsonPropertyName("onFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnFail { get; set; }

    public new static void RegisterTypeInfo<T>() where T : VectorGraphic
    {
        TouchComponent.RegisterTypeInfo<T>();
    }
}