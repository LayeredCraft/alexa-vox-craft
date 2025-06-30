using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class SetFocus : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(SetFocus);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ComponentId { get; set; }
}