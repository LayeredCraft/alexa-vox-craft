using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Select : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => "Select";

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Commands { get; set; }

    [JsonPropertyName("otherwise")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Otherwise { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? Data { get; set; }

    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<Select>(typeInfo =>
        {
            var otherwiseProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "otherwise");
            if (otherwiseProp is not null)
            {
                otherwiseProp.CustomConverter = new APLCommandListConverter(false);
            }

            var commandsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "commands");
            if (commandsProp is not null)
            {
                commandsProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}