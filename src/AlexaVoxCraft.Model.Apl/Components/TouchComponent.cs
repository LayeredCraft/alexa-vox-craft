using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class TouchComponent : ActionableComponent, IJsonSerializable<TouchComponent>
{
    [JsonPropertyName("gestures")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLGesture>> Gestures { get; set; }

    [JsonPropertyName("onCancel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnCancel { get; set; }

    [JsonPropertyName("onDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnDown { get; set; }

    [JsonPropertyName("onUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnUp { get; set; }

    [JsonPropertyName("onMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnMove { get; set; }

    [JsonPropertyName("onPress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnPress { get; set; }

    public static void RegisterTypeInfo<T>() where T : TouchComponent
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var gesturesProp = info.Properties.FirstOrDefault(p => p.Name == "gestures");
            if (gesturesProp is not null)
            {
                gesturesProp.CustomConverter = new APLGestureListConverter(false);
            }

            var onCancelProp = info.Properties.FirstOrDefault(p => p.Name == "onCancel");
            if (onCancelProp is not null)
            {
                onCancelProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onDownProp = info.Properties.FirstOrDefault(p => p.Name == "onDown");
            if (onDownProp is not null)
            {
                onDownProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onUpProp = info.Properties.FirstOrDefault(p => p.Name == "onUp");
            if (onUpProp is not null)
            {
                onUpProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onMove");
            if (onMoveProp is not null)
            {
                onMoveProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onPressProp = info.Properties.FirstOrDefault(p => p.Name == "onPress");
            if (onPressProp is not null)
            {
                onPressProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}