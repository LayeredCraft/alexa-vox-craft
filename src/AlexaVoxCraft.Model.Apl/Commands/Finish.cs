using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Finish : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Finish);
}