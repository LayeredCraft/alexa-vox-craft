using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AnimateItem : APLCommand, IJsonSerializable<AnimateItem>
{
    [JsonPropertyName("type")] public override string Type => nameof(AnimateItem);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("duration")] public APLValue<int?>? Duration { get; set; }

    [JsonPropertyName("easing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Easing { get; set; }

    [JsonPropertyName("repeatCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? RepeatCount { get; set; }

    [JsonPropertyName("repeatMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RepeatMode>? RepeatMode { get; set; }

    [JsonPropertyName("value")] public APLValue<IList<AnimatedProperty>> Value { get; set; }

    public static void RegisterTypeInfo<T>() where T : AnimateItem
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(typeInfo =>
        {
            var valueProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "value");
            if (valueProp is not null)
            {
                valueProp.CustomConverter = new AnimatedPropertyListConverter(false);
            }
        });
    }
}