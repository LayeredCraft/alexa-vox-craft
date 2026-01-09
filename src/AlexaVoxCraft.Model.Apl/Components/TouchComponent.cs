using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class TouchComponent : ActionableComponent, IJsonSerializable<TouchComponent>
{
    [JsonPropertyName("gestures")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLGesture> Gestures { get; set; }

    [JsonPropertyName("onCancel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnCancel { get; set; }

    [JsonPropertyName("onDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnDown { get; set; }

    [JsonPropertyName("onUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnUp { get; set; }

    [JsonPropertyName("onMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnMove { get; set; }

    [JsonPropertyName("onPress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnPress { get; set; }

    public static void RegisterTypeInfo<T>() where T : TouchComponent
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var gesturesProp = info.Properties.FirstOrDefault(p => p.Name == "gestures");
            gesturesProp?.CustomConverter = new APLGestureListConverter(false);

            var onCancelProp = info.Properties.FirstOrDefault(p => p.Name == "onCancel");
            onCancelProp?.CustomConverter = new APLCommandListConverter(false);

            var onDownProp = info.Properties.FirstOrDefault(p => p.Name == "onDown");
            onDownProp?.CustomConverter = new APLCommandListConverter(false);

            var onUpProp = info.Properties.FirstOrDefault(p => p.Name == "onUp");
            onUpProp?.CustomConverter = new APLCommandListConverter(false);

            var onMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onMove");
            onMoveProp?.CustomConverter = new APLCommandListConverter(false);

            var onPressProp = info.Properties.FirstOrDefault(p => p.Name == "onPress");
            onPressProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}