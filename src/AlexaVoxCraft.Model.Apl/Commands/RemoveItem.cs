using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class RemoveItem : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(RemoveItem);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }
}