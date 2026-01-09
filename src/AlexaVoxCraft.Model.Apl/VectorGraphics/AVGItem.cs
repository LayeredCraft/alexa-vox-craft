using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public abstract class AVGItem : IAVGItem
{
    [JsonPropertyName("type")] public abstract string Type { get; }

    [JsonPropertyName("filters")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<IAVGFilter> Filters { get; set; }
}