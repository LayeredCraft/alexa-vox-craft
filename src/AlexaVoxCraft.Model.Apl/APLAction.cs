using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLAction : IJsonSerializable<APLAction>
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Name { get; set; }

    [JsonPropertyName("label")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Label { get; set; }

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    public static void RegisterTypeInfo<T>() where T : APLAction
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            commandsProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}