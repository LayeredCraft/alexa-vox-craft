using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class OpenURL : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(OpenURL);

    [JsonPropertyName("source")] public APLValue<string> Source { get; set; }

    [JsonPropertyName("onFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnFail { get; set; }
}