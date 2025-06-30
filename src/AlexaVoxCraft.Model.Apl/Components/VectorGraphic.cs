using System.Collections.Generic;
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
    public APLValue<IList<APLCommand>>? OnLoad { get; set; }

    [JsonPropertyName("onFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnFail { get; set; }

    public new static void RegisterTypeInfo<T>() where T : VectorGraphic
    {
        TouchComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onLoadProp = info.Properties.FirstOrDefault(p => p.Name == "onLoad");
            if (onLoadProp is not null)
            {
                onLoadProp.CustomConverter = new APLCommandListConverter(true);
            }

            var onFailProp = info.Properties.FirstOrDefault(p => p.Name == "onFail");
            if (onFailProp is not null)
            {
                onFailProp.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}