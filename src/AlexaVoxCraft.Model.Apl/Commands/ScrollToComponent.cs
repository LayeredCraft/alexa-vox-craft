using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class ScrollToComponent : APLCommand
{
    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ItemAlignment>? Align { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("type")]
    public override string Type => nameof(ScrollToComponent);

    [JsonPropertyName("targetDuration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TargetDuration { get; set; }
}