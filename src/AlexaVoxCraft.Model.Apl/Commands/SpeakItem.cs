using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class SpeakItem : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(SpeakItem);

    [JsonPropertyName("align")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ItemAlignment?>? Align { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("highlightMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<HighlightMode?>? HighlightMode { get; set; }
}