using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class ResolutionAuthority
{
    [JsonPropertyName("authority")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("status")]
    public ResolutionStatus Status { get; set; } = null!;

    [JsonPropertyName("values"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResolutionValueContainer[]? Values { get; set; }
}