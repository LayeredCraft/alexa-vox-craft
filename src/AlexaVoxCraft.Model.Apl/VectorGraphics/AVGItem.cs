using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public abstract class AVGItem : IAVGItem, IJsonSerializable<AVGItem>
{
    [JsonPropertyName("type")] public abstract string Type { get; }

    [JsonPropertyName("filters")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<IAVGFilter>> Filters { get; set; }

    public static void RegisterTypeInfo<T>() where T : AVGItem
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var filtersProp = info.Properties.FirstOrDefault(p => p.Name == "filters");
            if (filtersProp is not null)
            {
                filtersProp.CustomConverter = new AvgFilterListConverter(false);
            }
        });
    }
}