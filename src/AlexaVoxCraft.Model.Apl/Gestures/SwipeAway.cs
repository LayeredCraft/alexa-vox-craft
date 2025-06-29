using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class SwipeAway : APLGesture, IJsonSerializable<SwipeAway>
{
    [JsonPropertyName("type")] public override string Type => nameof(SwipeAway);

    [JsonPropertyName("onSwipeMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnSwipeMove { get; set; }

    [JsonPropertyName("onSwipeDone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnSwipeDone { get; set; }

    [JsonPropertyName("item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public APLValue<IList<APLComponent>>? Item { get; set; }

    [JsonPropertyName("action")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SwipeAction?>? Action { get; set; }

    [JsonPropertyName("direction")] public APLValue<SwipeDirection> Direction { get; set; }

    public static void RegisterTypeInfo<T>() where T : SwipeAway
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onSwipeMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeMove");
            if (onSwipeMoveProp is not null)
            {
                onSwipeMoveProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onSwipeDoneProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeDone");
            if (onSwipeDoneProp is not null)
            {
                onSwipeDoneProp.CustomConverter = new APLCommandListConverter(false);
            }

            var itemProp = info.Properties.FirstOrDefault(p => p.Name == "item");
            if (itemProp is not null)
            {
                itemProp.CustomConverter = new APLComponentListConverter(false);
            }
        });
    }
}