using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class ScrollView : ActionableComponent, IJsonSerializable<ScrollView>
{
    public const string ComponentType = "ScrollView";
    [JsonPropertyName("type")]
    public override string Type => ComponentType;

    public ScrollView()
    {
    }

    public ScrollView(APLComponent component)
    {
        Item = [component];
    }

    public ScrollView(params APLComponent[] components) : this((IEnumerable<APLComponent>)components)
    {
    }

    public ScrollView(IEnumerable<APLComponent> components)
    {
        Item = [..components];
    }

    [JsonPropertyName("item")][JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent> Item { get; set; }

    [JsonPropertyName("onScroll")][JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnScroll { get; set; }

    public new static void RegisterTypeInfo<T>() where T : ScrollView
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemProp = info.Properties.FirstOrDefault(p => p.Name == "item");
            itemProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            onScrollProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}