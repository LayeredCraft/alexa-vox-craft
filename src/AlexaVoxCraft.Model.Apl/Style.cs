using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class Style : IJsonSerializable<Style>
{
    [JsonPropertyName("extend")]
    public IList<string> Extends { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [JsonIgnore]
    public StyleValue Value
    {
        get => Values?.FirstOrDefault();
        set { Values = new List<StyleValue> { Value }; }
    }

    [JsonPropertyName("values")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<StyleValue>? Values { get; set; }

    public static void RegisterTypeInfo<T>() where T : Style
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var valuesProp = info.Properties.FirstOrDefault(p => p.Name == "values");
            if (valuesProp is not null)
            {
                valuesProp.ShouldSerialize = (obj, _) =>
                {
                    var style = (T)obj;
                    return style.Values?.Any() ?? false;
                };
                valuesProp.CustomConverter = new GenericSingleOrListConverter<StyleValue>(false);
            }
            var extendProp = info.Properties.FirstOrDefault(p => p.Name == "extend");
            if (extendProp is not null)
            {
                extendProp.ShouldSerialize = (obj, _) =>
                {
                    var style = (T)obj;
                    return style.Extends?.Any() ?? false;
                };
                extendProp.CustomConverter = new StringOrArrayConverter(false);
            }
        });
    }
}