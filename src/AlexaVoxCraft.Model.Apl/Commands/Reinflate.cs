using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Reinflate : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Reinflate);

    [JsonPropertyName("preservedSequencers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<string>? PreservedSequencers { get; set; }
}