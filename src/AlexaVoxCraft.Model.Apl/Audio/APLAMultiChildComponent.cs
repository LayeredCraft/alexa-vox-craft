using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public abstract class APLAMultiChildComponent : APLAComponent, IJsonSerializable<APLAMultiChildComponent>
{
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLAComponent>? Items { get; set; }

    public static void RegisterTypeInfo<T>() where T : APLAMultiChildComponent
    {
        AlexaJsonOptions.RegisterTypeModifier<APLAMultiChildComponent>(typeInfo =>
        {
            var dataProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "data");
            dataProp?.CustomConverter = new APLValueCollectionConverter<object>(false);

            var itemsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.CustomConverter = new APLValueCollectionConverter<APLAComponent>(false);
        });
    }
}