using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class VisibilityChangeHandler : IJsonSerializable<VisibilityChangeHandler>
{
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> When { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Description { get; set; }

    [JsonPropertyName("commands")] public APLValue<IList<APLCommand>> Commands { get; set; }

    public static void RegisterTypeInfo<T>() where T : VisibilityChangeHandler
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == "commands");
            if (prop is not null)
            {
                prop.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}