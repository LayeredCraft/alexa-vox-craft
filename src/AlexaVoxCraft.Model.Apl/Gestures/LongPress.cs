using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class LongPress : APLGesture, IJsonSerializable<LongPress>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(LongPress);

    [JsonPropertyName("onLongPressStart")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnLongPressStart { get; set; }

    [JsonPropertyName("onLongPressEnd")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnLongPressEnd { get; set; }

    public static void RegisterTypeInfo<T>() where T : LongPress
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onLongPressStartProp = info.Properties.FirstOrDefault(p => p.Name == "onLongPressStart");
            if (onLongPressStartProp is not null)
            {
                onLongPressStartProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onLongPressEndProp = info.Properties.FirstOrDefault(p => p.Name == "onLongPressEnd");
            if (onLongPressEndProp is not null)
            {
                onLongPressEndProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}