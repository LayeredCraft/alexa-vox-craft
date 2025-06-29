using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Scroll : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Scroll);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("distance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Distance { get; set; }

    [JsonPropertyName("targetDuration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TargetDuration { get; set; }
}