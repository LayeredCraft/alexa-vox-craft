using System.Collections.Generic;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

[JsonConverter(typeof(AVGItemConverter))]
public interface IAVGItem
{
    [JsonPropertyName("type")]
    string Type { get; }

    [JsonPropertyName("filters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<IAVGFilter>>? Filters { get; set; }
}