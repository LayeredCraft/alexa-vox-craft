using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class CommandResult
{
    [JsonPropertyName("deviceId")] public string DeviceId { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; }

    [JsonPropertyName("type")] public CommandResultType Type { get; set; }
}