using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class InsertItem : APLCommand, IJsonSerializable<InsertItem>
{
    [JsonPropertyName("type")] public override string Type => nameof(InsertItem);

    [JsonPropertyName("at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? At { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? Items { get; set; }

    public static void RegisterTypeInfo<T>() where T : InsertItem
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(typeInfo =>
        {
            var parameterProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "items");
            if (parameterProp is not null)
            {
                parameterProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }
        });
    }
}