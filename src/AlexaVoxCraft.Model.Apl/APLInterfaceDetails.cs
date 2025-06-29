using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLInterfaceDetails
{
    [JsonPropertyName("runtime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLInterfaceRuntime Runtime { get; set; }
}