using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLKeyboardHandler
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("propagate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Propagate { get; set; }

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Commands { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<APLKeyboardHandler>(info =>
        {
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            if (commandsProp is not null)
            {
                commandsProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}