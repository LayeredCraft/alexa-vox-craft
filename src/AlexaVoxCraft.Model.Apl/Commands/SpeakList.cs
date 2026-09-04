using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class SpeakList : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(SpeakList);

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ItemAlignment?> Align { get; set; } = null!;

    [JsonPropertyName("componentId")] public APLValue<string> ComponentId { get; set; } = null!;

    [JsonPropertyName("start")] public APLValue<int> Start { get; set; } = null!;

    [JsonPropertyName("count")] public APLValue<int> Count { get; set; } = null!;

    [JsonPropertyName("minimumDwellTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? MinimumDwellTime { get; set; }
}