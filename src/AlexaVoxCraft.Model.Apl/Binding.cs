using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class Binding : IJsonSerializable<Binding>
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("value")] public object Value { get; set; }

    [JsonPropertyName("type")] public ParameterType Type { get; set; } = ParameterType.any;

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? Commands { get; set; }

    [JsonConstructor]
    private Binding()
    {
    }

    public Binding(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static void RegisterTypeInfo<T>() where T : Binding
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var typeProp = info.Properties.FirstOrDefault(p => p.Name == "type");
            if (typeProp is not null)
            {
                typeProp.ShouldSerialize = (obj, _) =>
                {
                    var binding = (T)obj;
                    return binding.Type != ParameterType.any;
                };
            }
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            if (commandsProp is not null)
            {
                commandsProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}