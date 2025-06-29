using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class SetPage:APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(SetPage);

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SetPagePosition>? Position { get; set; }

    [JsonPropertyName("value")]
    public APLValue<int> Value { get; set; }

    [JsonPropertyName("transitionDuration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TransitionDuration { get; set; }
}