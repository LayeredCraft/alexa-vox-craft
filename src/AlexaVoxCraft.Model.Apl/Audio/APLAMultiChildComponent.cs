using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public abstract class APLAMultiChildComponent : APLAComponent
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? Data { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLAComponent>>? Items { get; set; }

    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<APLAMultiChildComponent>(typeInfo =>
        {
            var dataProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "data");
            if (dataProp is not null)
            {
                dataProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }

            var itemsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "items");
            if (itemsProp is not null)
            {
                itemsProp.CustomConverter = new APLAComponentListConverter(false);
            }
        });
    }
}