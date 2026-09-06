using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class Usage
{
    [JsonPropertyName("instanceId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string InstanceId { get; set; } = null!;

    [JsonPropertyName("location")] public string Location { get; set; } = null!;
}