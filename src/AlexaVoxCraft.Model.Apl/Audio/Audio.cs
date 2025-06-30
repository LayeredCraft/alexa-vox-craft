using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Audio : APLAComponent
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Audio);

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Source { get; set; }

    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLAFilter>>? Filters { get; set; }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<Audio>(typeInfo =>
        {
            var filterProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "filter");
            if (filterProp is not null)
            {
                filterProp.CustomConverter = new APLAFilterListConverter(false);
            }
        });
    }
}