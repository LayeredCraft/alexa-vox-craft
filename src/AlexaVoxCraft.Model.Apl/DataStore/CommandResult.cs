using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class CommandResult
{
    [JsonPropertyName("deviceId")] public string DeviceId { get; set; } = null!;

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; } = null!;

    [JsonPropertyName("type")] public CommandResultType Type { get; set; }
}