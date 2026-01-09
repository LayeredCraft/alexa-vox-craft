using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class DoublePress : APLGesture, IJsonSerializable<DoublePress>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(DoublePress);

    [JsonPropertyName("onDoublePress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnDoublePress { get; set; }

    [JsonPropertyName("onSinglePress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnSinglePress { get; set; }

    public static void RegisterTypeInfo<T>() where T : DoublePress
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onDoublePressProp = info.Properties.FirstOrDefault(p => p.Name == "onDoublePress");
            if (onDoublePressProp is not null)
            {
                onDoublePressProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onSinglePressProp = info.Properties.FirstOrDefault(p => p.Name == "onSinglePress");
            if (onSinglePressProp is not null)
            {
                onSinglePressProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}