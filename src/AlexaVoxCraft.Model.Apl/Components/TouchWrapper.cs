using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class TouchWrapper : TouchComponent, IJsonSerializable<TouchWrapper>
{
    public TouchWrapper()
    {
    }

    public TouchWrapper(APLComponent item)
    {
        Item = [item];
    }

    public TouchWrapper(params APLComponent[] item) : this((IEnumerable<APLComponent>)item)
    {
    }

    public TouchWrapper(IEnumerable<APLComponent> item)
    {
        Item = [..item];
    }

    [JsonPropertyName("type")] public override string Type => nameof(TouchWrapper);


    [JsonPropertyName("item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? Item { get; set; }

    public static void RegisterTypeInfo<T>() where T : TouchWrapper
    {
        TouchComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemProp = info.Properties.FirstOrDefault(p => p.Name == "item");
            itemProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);
        });
    }
}