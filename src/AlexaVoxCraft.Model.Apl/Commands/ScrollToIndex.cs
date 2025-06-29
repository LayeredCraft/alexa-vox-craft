using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class ScrollToIndex : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(ScrollToIndex);

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ItemAlignment?>? Align { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("index")] public APLValue<int> Index { get; set; }

    [JsonPropertyName("targetDuration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TargetDuration { get; set; }
}