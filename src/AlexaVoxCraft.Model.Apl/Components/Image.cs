using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Filters;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Image : APLComponent, IJsonSerializable<Image>
{
    public Image()
    {
    }

    public Image(params string[] sources)
    {
        Sources = sources;
    }

    public const string ComponentType = "Image";
    [JsonPropertyName("type")]
    public override string Type => ComponentType;

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Align { get; set; }

    [JsonPropertyName("borderRadius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue BorderRadius { get; set; }

    [JsonPropertyName("overlayGradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<APLGradient> OverlayGradient { get; set; }

    [JsonPropertyName("overlayColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> OverlayColor { get; set; }

    [JsonPropertyName("scale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?> Scale { get; set; }

    [JsonPropertyName("sources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<string>>? Sources { get; set; }

    [JsonPropertyName("filters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<IImageFilter>> Filters { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Image
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var sourcesProp = info.Properties.FirstOrDefault(p => p.Name == "sources");
            if (sourcesProp is not null)
            {
                sourcesProp.CustomConverter = new StringOrArrayConverter(false);
            }

            var filtersProp = info.Properties.FirstOrDefault(p => p.Name == "filters");
            if (filtersProp is not null)
            {
                filtersProp.CustomConverter = new ImageFilterListConverter(false);
            }
        });
    }
}