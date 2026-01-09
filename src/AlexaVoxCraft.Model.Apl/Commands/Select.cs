using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Select : APLCommand, IJsonSerializable<Select>
{
    [JsonPropertyName("type")]
    public override string Type => "Select";

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    [JsonPropertyName("otherwise")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Otherwise { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data { get; set; }

    public static void RegisterTypeInfo<T>() where T : Select
    {
        AlexaJsonOptions.RegisterTypeModifier<Select>(typeInfo =>
        {
            var otherwiseProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "otherwise");
            otherwiseProp?.CustomConverter = new APLCommandListConverter(false);

            var commandsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "commands");
            commandsProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}