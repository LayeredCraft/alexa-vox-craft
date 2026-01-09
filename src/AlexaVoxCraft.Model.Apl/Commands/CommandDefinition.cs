using System.Collections.Generic;
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
            var parameterProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "parameters");
            if (parameterProp is not null)
            {
                parameterProp.CustomConverter = new ParameterListConverter(true);
            }
            var commandsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "commands");
            if (commandsProp is not null)
            {
                commandsProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}