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
        Item = new List<APLComponent> { component };
    }

    public ScrollView(params APLComponent[] components) : this((IEnumerable<APLComponent>)components)
    {
    }

    public ScrollView(IEnumerable<APLComponent> components)
    {
        Item = new List<APLComponent>(components);
    }

    [JsonPropertyName("item")][JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLComponent>> Item { get; set; }

    [JsonPropertyName("onScroll")][JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnScroll { get; set; }

    public new static void RegisterTypeInfo<T>() where T : ScrollView
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemProp = info.Properties.FirstOrDefault(p => p.Name == "item");
            if (itemProp is not null)
            {
                itemProp.CustomConverter = new APLComponentListConverter(false);
            }

            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            if (onScrollProp is not null)
            {
                onScrollProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}