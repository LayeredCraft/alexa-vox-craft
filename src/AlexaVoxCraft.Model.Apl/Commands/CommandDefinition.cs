using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class CommandDefinition : IJsonSerializable<CommandDefinition>
{
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<Parameter>? Parameters { get; set; }

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    public static void RegisterTypeInfo<T>() where T : CommandDefinition
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(typeInfo =>
        {
            var parametersProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "parameters");
            parametersProp?.CustomConverter = new ParameterValueCollectionConverter(true);
            var commandsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "commands");
            commandsProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}