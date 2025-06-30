using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Idle : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(Idle);
}