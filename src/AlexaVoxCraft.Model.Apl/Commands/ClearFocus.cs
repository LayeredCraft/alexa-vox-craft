using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class ClearFocus : APLCommand
{
    [JsonPropertyName("type")]
    public override string Type => nameof(ClearFocus);

}