using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AutoPage : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(AutoPage);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Count { get; set; }

    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Duration { get; set; }

    [JsonPropertyName("transitionDuration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TransitionDuration { get; set; }
}