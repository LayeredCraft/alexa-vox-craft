using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class ControlMedia : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(ControlMedia);

    [JsonPropertyName("command")] public APLValue<ControlMediaCommand> Command { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Value { get; set; }

}