using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class SetValue : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(SetValue);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("property")]
    public APLValue<string> Property { get; set; }


    [JsonPropertyName("value")]
    public APLValue<object> Value { get; set; }
}