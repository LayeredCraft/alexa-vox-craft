using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class TickHandler : IJsonSerializable<TickHandler>
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("minimumDelay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int>? MinimumDelay { get; set; }

    [JsonPropertyName("commands")] public APLValue<IList<APLCommand>> Commands { get; set; }

    public static void RegisterTypeInfo<T>() where T : TickHandler
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            if (primaryActionProp is not null)
            {
                primaryActionProp.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}