using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVG
{
    [JsonPropertyName("type")] public string Type => nameof(AVG);

    [JsonIgnore]
    public AVGVersion Version
    {
        get => EnumParser.ToEnum(VersionString, AVGVersion.Unknown);
        set => VersionString = EnumParser.ToEnumString(typeof(AVGVersion), value);
    }

    [JsonPropertyName("version")] public string VersionString { get; set; } = "1.2";


    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? Data { get; set; }

    [JsonPropertyName("height")] public APLAbsoluteDimensionValue Height { get; set; }

    [JsonPropertyName("width")] public APLAbsoluteDimensionValue Width { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<IAVGItem>>? Items { get; set; }

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<AVGResource>? Resources { get; set; }

    [JsonPropertyName("styles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Style>? Styles { get; set; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<AVGParameter>>? Parameters { get; set; }

    [JsonPropertyName("scaleTypeHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AVGScaleType>? ScaleTypeHeight { get; set; }

    [JsonPropertyName("scaleTypeWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AVGScaleType>? ScaleTypeWidth { get; set; }

    [JsonPropertyName("viewportHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ViewportHeight { get; set; }

    [JsonPropertyName("viewportWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ViewportWidth { get; set; }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<AVG>(info =>
        {
            var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
            if (dataProp is not null)
            {
                dataProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }
            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            if (itemsProp is not null)
            {
                itemsProp.CustomConverter = new AVGItemListConverter(false);
            }
        });
    }
}