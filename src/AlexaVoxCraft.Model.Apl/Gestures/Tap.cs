using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public class Tap : APLGesture, IJsonSerializable<Tap>
{
    [JsonPropertyName("type")] public override string Type => nameof(Tap);

    [JsonPropertyName("onTap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>> OnTap { get; set; }

    public static void RegisterTypeInfo<T>() where T : Tap
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onTapProp = info.Properties.FirstOrDefault(p => p.Name == "onTap");
            if (onTapProp is not null)
            {
                onTapProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}