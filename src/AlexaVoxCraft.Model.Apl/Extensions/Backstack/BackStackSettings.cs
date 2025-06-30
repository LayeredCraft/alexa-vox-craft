using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.Backstack;

public class BackStackSettings
{
    [JsonPropertyName("backstackId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string BackstackId { get; set; }
}