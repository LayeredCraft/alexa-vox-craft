using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PostalAddress
{
    [JsonPropertyName("@type")] public string Type => "PostalAddress";
    [JsonPropertyName("@version")] public string Version => 1.ToString();
    [JsonPropertyName("streetAddress")] public string StreetAddress { get; set; } = null!;
    [JsonPropertyName("locality")] public string Locality { get; set; } = null!;
    [JsonPropertyName("region")] public string Region { get; set; } = null!;
    [JsonPropertyName("postalCode")] public string PostalCode { get; set; } = null!;
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Country { get; set; } = null!;
}