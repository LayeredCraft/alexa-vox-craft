using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class Selector : APLAMultiChildComponent
{
    [JsonPropertyName("strategy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SelectorStrategy?>? Strategy { get; set; }

    [JsonPropertyName("type")]
    public override string Type => nameof(Selector);

    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<Selector>(typeInfo =>
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